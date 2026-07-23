using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Domain.Entities;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Projects;

public sealed record AddProjectMemberRequest(List<AddProjectMemberItem> Members);

public sealed record AddProjectMemberItem(Guid UserId, ProjectRole? Role);

public sealed record AddProjectMemberCommand(
    Guid UserId,
    Guid ProjectId,
    List<AddProjectMemberItem> Members,
    CancellationToken CancellationToken
) : IRequest;

public sealed class AddProjectMemberCommandHandler : IRequestHandler<AddProjectMemberCommand>
{
    private readonly IAppDbContext _context;

    public AddProjectMemberCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(AddProjectMemberCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = request.UserId;
        var projectId = request.ProjectId;

        // Get project info including team and creator
        var project = await _context
            .Projects.AsNoTracking()
            .Where(p => p.Id == projectId)
            .Select(p => new { p.TeamId, p.CreatorId })
            .FirstOrDefaultAsync(cancellationToken);

        if (project is null)
            throw new NotFoundException(ErrorCodes.ProjectNotFound);

        // Authorization: current user must be Admin or CoAdmin of the team
        var isAdminOrCoAdmin = await _context
            .TeamMembers.AsNoTracking()
            .AnyAsync(
                tm =>
                    tm.TeamId == project.TeamId
                    && tm.UserId == currentUserId
                    && (tm.Role == TeamMemberRole.Admin || tm.Role == TeamMemberRole.CoAdmin),
                cancellationToken
            );

        if (!isAdminOrCoAdmin)
            throw new ForbiddenException(ErrorCodes.Forbidden);

        // Get current user's team role to enforce CoAdmin restrictions
        var currentUserTeamRole = await _context
            .TeamMembers.AsNoTracking()
            .Where(tm => tm.TeamId == project.TeamId && tm.UserId == currentUserId)
            .Select(tm => tm.Role)
            .FirstAsync(cancellationToken);

        var isAdmin = currentUserTeamRole == TeamMemberRole.Admin;

        // Get existing project member IDs to avoid duplicates
        var existingMemberIds = await _context
            .ProjectMembers.AsNoTracking()
            .Where(pm => pm.ProjectId == projectId)
            .Select(pm => pm.UserId)
            .ToListAsync(cancellationToken);

        var existingSet = new HashSet<Guid>(existingMemberIds);
        var now = DateTimeOffset.UtcNow;
        var newMembers = new List<ProjectMember>();

        foreach (var item in request.Members)
        {
            // Skip if user is already a project member
            if (existingSet.Contains(item.UserId))
                continue;

            // CoAdmin can only add members with role Member
            var role = item.Role ?? ProjectRole.Member;
            if (!isAdmin && role != ProjectRole.Member)
            {
                throw new ForbiddenException(ErrorCodes.Forbidden);
            }

            // If the user being added is the project creator, force Leader role
            if (item.UserId == project.CreatorId)
            {
                role = ProjectRole.Leader;
            }

            newMembers.Add(
                new ProjectMember
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    UserId = item.UserId,
                    Role = role,
                    JoinedAt = now,
                }
            );

            // Add to existing set to prevent duplicate within the same batch
            existingSet.Add(item.UserId);
        }

        if (newMembers.Count > 0)
        {
            _context.ProjectMembers.AddRange(newMembers);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
