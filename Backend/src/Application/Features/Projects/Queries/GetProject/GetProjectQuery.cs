using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Domain.Entities;
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
    string CreatorName,
    string TeamName,
    string Visibility,
    bool CanView,
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
        var project =
            await _context
                .Projects.AsNoTracking()
                .Where(p => p.Id == request.Id)
                .Include(p => p.Creator)
                .Include(p => p.Team)
                .Include(p => p.ProjectMembers)
                .Include(p => p.TaskItems)
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(ErrorCodes.NotFound);

        // Check if user is a member of the project's team
        var isTeamMember = await _context
            .TeamMembers.AsNoTracking()
            .AnyAsync(
                tm => tm.TeamId == project.TeamId && tm.UserId == request.CurrentUserId,
                cancellationToken
            );

        if (!isTeamMember)
        {
            throw new NotFoundException(ErrorCodes.NotFound);
        }

        // Get user's team role for CanView calculation
        var teamRole = await _context
            .TeamMembers.AsNoTracking()
            .Where(tm => tm.TeamId == project.TeamId && tm.UserId == request.CurrentUserId)
            .Select(tm => tm.Role)
            .FirstAsync(cancellationToken);

        var isTeamAdminOrCoAdmin = teamRole is TeamMemberRole.Admin or TeamMemberRole.CoAdmin;
        var hasProjectMembership = project.ProjectMembers.Any(pm =>
            pm.UserId == request.CurrentUserId
        );
        var canView = isTeamAdminOrCoAdmin || hasProjectMembership;

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
            project.Creator.Name,
            project.Team.Name,
            project.Visibility.ToString(),
            canView,
            project.ProjectMembers.Count,
            project.TaskItems.Count(t => t.Status == TaskItemStatus.Done),
            project.TaskItems.Count
        );
    }
}
