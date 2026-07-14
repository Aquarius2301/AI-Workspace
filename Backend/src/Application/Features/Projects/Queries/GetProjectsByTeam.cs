using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Application.Common.Models;
using AIWorkspace.Application.Helpers;
using AIWorkspace.Domain.Entities;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Projects;

public sealed record GetProjectsByTeamQuery(
    Guid CurrentUserId,
    Guid TeamId,
    string? Search,
    ProjectVisibility? Visibility,
    PaginationRequest Pagination,
    CancellationToken CancellationToken = default
) : IRequest<PaginationResult<ProjectItem>>;

public sealed record ProjectItem(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    string CreatorName,
    string Visibility,
    bool CanView,
    int MemberCount,
    int CompletedTaskCount,
    int TotalTaskCount
);

public sealed class GetProjectsByTeamQueryHandler
    : IRequestHandler<GetProjectsByTeamQuery, PaginationResult<ProjectItem>>
{
    private readonly IAppDbContext _context;

    public GetProjectsByTeamQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginationResult<ProjectItem>> Handle(
        GetProjectsByTeamQuery request,
        CancellationToken cancellationToken
    )
    {
        var currentUserId = request.CurrentUserId;
        var teamId = request.TeamId;
        var search = request.Search;
        var visibility = request.Visibility;
        var pagination = request.Pagination;

        // Verify the current user is a member of this team and get their role
        var teamMembership = await _context
            .TeamMembers.AsNoTracking()
            .FirstOrDefaultAsync(
                tm => tm.TeamId == teamId && tm.UserId == currentUserId,
                cancellationToken
            );

        if (teamMembership is null)
        {
            throw new NotFoundException(ErrorCodes.TeamNotFound);
        }

        var isTeamAdminOrCoAdmin =
            teamMembership.Role is TeamMemberRole.Admin or TeamMemberRole.CoAdmin;

        // Fetch all projects for the team with related data
        var projects = await _context
            .Projects.AsNoTracking()
            .Where(p => p.TeamId == teamId)
            .Include(p => p.Creator)
            .Include(p => p.ProjectMembers)
            .Include(p => p.TaskItems)
            .ToListAsync(cancellationToken);

        // Build the result list with CanView computed for each project
        var projectResults = projects
            .Select(p =>
            {
                var hasProjectMembership = p.ProjectMembers.Any(pm => pm.UserId == currentUserId);
                var canView = isTeamAdminOrCoAdmin || hasProjectMembership;

                return new
                {
                    Project = p,
                    CanView = canView,
                    ShouldShow = p.Visibility == ProjectVisibility.Public || canView,
                };
            })
            .ToList();

        // Apply show filter: only include projects that ShouldShow
        var filtered = projectResults.Where(x => x.ShouldShow).ToList();

        // Apply search filter on project name
        if (!string.IsNullOrWhiteSpace(search))
        {
            filtered = filtered
                .Where(x =>
                    CollationSearchHelper.Contains(x.Project.Name, search)
                    || CollationSearchHelper.Contains(x.Project.Description, search)
                )
                .ToList();
        }

        // Apply visibility filter if provided
        if (visibility.HasValue)
        {
            filtered = filtered.Where(x => x.Project.Visibility == visibility.Value).ToList();
        }

        // Get total count after filtering
        var total = filtered.Count;

        // Apply pagination
        var pagedResults = filtered
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToList();

        // Project to result items
        var items = pagedResults
            .Select(x => new ProjectItem(
                x.Project.Id,
                x.Project.Name,
                x.Project.Slug,
                x.Project.Description,
                x.Project.Creator.Name,
                x.Project.Visibility.ToString(),
                x.CanView,
                x.Project.ProjectMembers.Count,
                x.Project.TaskItems.Count(t => t.Status == TaskItemStatus.Done),
                x.Project.TaskItems.Count
            ))
            .ToList();

        return new PaginationResult<ProjectItem>(
            pagination.Page,
            pagination.PageSize,
            total,
            items
        );
    }
}
