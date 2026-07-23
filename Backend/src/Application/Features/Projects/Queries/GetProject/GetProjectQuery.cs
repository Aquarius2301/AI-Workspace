using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Projects;

public sealed record GetProjectQuery(
    Guid Id,
    Guid CurrentUserId,
    CancellationToken CancellationToken
) : IRequest<GetProjectResult>;

public sealed record GetProjectResult(
    Guid Id,
    string Name,
    string? Description,
    string Slug,
    Guid CreatorId,
    string CreatorName,
    string TeamName,
    string Visibility,
    bool CanView,
    bool CanEdit,
    bool CanAddMember,
    int MemberCount,
    int CompletedTaskCount,
    int TotalTaskCount
);

public sealed class GetProjectQueryHandler : IRequestHandler<GetProjectQuery, GetProjectResult>
{
    private readonly IAppDbContext _context;

    public GetProjectQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<GetProjectResult> Handle(
        GetProjectQuery request,
        CancellationToken cancellationToken
    )
    {
        var projectId = request.Id;
        var currentUserId = request.CurrentUserId;

        // Fetch project with a single projection — subqueries for counts, no full table loads
        var project =
            await _context
                .Projects.AsNoTracking()
                .Where(p => p.Id == projectId)
                .Select(p => new
                {
                    p.Id,
                    p.CreatorId,
                    p.Name,
                    p.Description,
                    p.Slug,
                    p.TeamId,
                    p.Visibility,
                    CreatorName = p.Creator.Name,
                    TeamName = p.Team.Name,
                    MemberCount = p.ProjectMembers.Count,
                    CompletedTaskCount = p.TaskItems.Count(t => t.Status == TaskItemStatus.Done),
                    TotalTaskCount = p.TaskItems.Count,
                })
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(ErrorCodes.NotFound);

        // Check if user is a member of the project's team
        var isTeamMember = await _context
            .TeamMembers.AsNoTracking()
            .AnyAsync(
                tm => tm.TeamId == project.TeamId && tm.UserId == currentUserId,
                cancellationToken
            );

        if (!isTeamMember)
        {
            throw new NotFoundException(ErrorCodes.NotFound);
        }

        // Get user's team role in one query
        var teamRole = await _context
            .TeamMembers.AsNoTracking()
            .Where(tm => tm.TeamId == project.TeamId && tm.UserId == currentUserId)
            .Select(tm => tm.Role)
            .FirstAsync(cancellationToken);

        var isTeamAdminOrCoAdmin = teamRole is TeamMemberRole.Admin or TeamMemberRole.CoAdmin;

        // Check project membership in a single DB query (not in-memory)
        var hasProjectMembership = await _context
            .ProjectMembers.AsNoTracking()
            .AnyAsync(
                pm => pm.ProjectId == projectId && pm.UserId == currentUserId,
                cancellationToken
            );

        var canView = isTeamAdminOrCoAdmin || hasProjectMembership;

        // Check if user can edit
        var isProjectLeader = await _context
            .ProjectMembers.AsNoTracking()
            .AnyAsync(
                pm =>
                    pm.ProjectId == projectId
                    && pm.UserId == currentUserId
                    && pm.Role == ProjectRole.Leader,
                cancellationToken
            );

        var canEdit = isTeamAdminOrCoAdmin || isProjectLeader;

        // If private and user can't view, throw
        if (project.Visibility == ProjectVisibility.Private && !canView)
        {
            throw new NotFoundException(ErrorCodes.NotFound);
        }

        return new GetProjectResult(
            project.Id,
            project.Name,
            project.Description,
            project.Slug,
            project.CreatorId,
            project.CreatorName,
            project.TeamName,
            project.Visibility.ToString(),
            canView,
            canEdit,
            isTeamAdminOrCoAdmin,
            project.MemberCount,
            project.CompletedTaskCount,
            project.TotalTaskCount
        );
    }
}
