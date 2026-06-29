using BusinessObject.Entities;
using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Common.Models;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Teams;

public sealed record GetTeamProjectsQuery(
    Guid CurrentUserId,
    Guid TeamId,
    string? Search,
    ProjectVisibility? Visibility,
    PaginationRequest Pagination,
    CancellationToken CancellationToken = default
) : IRequest<PaginationResult<TeamProjectItem>>;

public sealed record TeamProjectItem(
    Guid Id,
    string Name,
    string? Description,
    string CreatorName,
    string Visibility,
    bool CanView,
    int MemberCount,
    int CompletedTaskCount,
    int TotalTaskCount
);

public sealed class GetTeamProjectsQueryHandler
    : IRequestHandler<GetTeamProjectsQuery, PaginationResult<TeamProjectItem>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTeamProjectsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginationResult<TeamProjectItem>> Handle(
        GetTeamProjectsQuery request,
        CancellationToken cancellationToken
    )
    {
        // Step 1: Check if user is a member of the team and get their role
        var teamMember =
            await _unitOfWork
                .TeamMembers.ReadOnly()
                .FirstOrDefaultAsync(
                    tm => tm.TeamId == request.TeamId && tm.UserId == request.CurrentUserId,
                    cancellationToken
                )
            ?? throw new ForbiddenException(ErrorCodes.NotTeamMember);

        var userRole = teamMember.Role;

        // Step 2: Query all projects of the team with search filter
        var projectsQuery = _unitOfWork.Projects.ReadOnly().Where(p => p.TeamId == request.TeamId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchLower = request.Search.ToLower();
            projectsQuery = projectsQuery.Where(p => p.Name.ToLower().Contains(searchLower));
        }

        if (request.Visibility.HasValue)
        {
            projectsQuery = projectsQuery.Where(p => p.Visibility == request.Visibility.Value);
        }

        var allProjects = await projectsQuery
            .Include(p => p.Creator)
            .Include(p => p.ProjectMembers)
            .Include(p => p.TaskItems)
            .ToListAsync(cancellationToken);

        // Step 3: Get the list of project IDs where the current user is a member
        var userProjectIds = allProjects
            .Where(p => p.ProjectMembers.Any(pm => pm.UserId == request.CurrentUserId))
            .Select(p => p.Id)
            .ToHashSet();

        // Step 4: Filter by visibility (show condition) and map to DTO
        var showableProjects = allProjects
            .Where(p => ShouldShow(p, userRole, userProjectIds.Contains(p.Id)))
            .Select(p => new TeamProjectItem(
                p.Id,
                p.Name,
                p.Description,
                p.Creator.Name,
                p.Visibility.ToString(),
                CanViewProject(userRole, userProjectIds.Contains(p.Id)),
                p.ProjectMembers.Count,
                p.TaskItems.Count(t => t.Status == TaskItemStatus.Done),
                p.TaskItems.Count
            ))
            .OrderBy(p => p.CanView ? 0 : 1) // Prioritize projects that can be viewed
            .ThenBy(p => p.Name) // Then order by name
            .ToList();

        var total = showableProjects.Count;

        var pagedItems = showableProjects
            .Skip((request.Pagination.Page - 1) * request.Pagination.PageSize)
            .Take(request.Pagination.PageSize)
            .ToList();

        return new PaginationResult<TeamProjectItem>(
            request.Pagination.Page,
            request.Pagination.PageSize,
            total,
            pagedItems
        );
    }

    private static bool ShouldShow(Project project, TeamMemberRole role, bool isProjectMember)
    {
        // Admin can see all projects
        if (role == TeamMemberRole.Admin)
            return true;

        // Public projects are visible to all team members
        if (project.Visibility == ProjectVisibility.Public)
            return true;

        // Private projects are only visible if user is a project member
        if (project.Visibility == ProjectVisibility.Private && isProjectMember)
            return true;

        return false;
    }

    private static bool CanViewProject(TeamMemberRole role, bool isProjectMember)
    {
        // Admin can view all project details
        if (role == TeamMemberRole.Admin)
            return true;

        // Leader/Member can only view if they are a project member
        return isProjectMember;
    }
}
