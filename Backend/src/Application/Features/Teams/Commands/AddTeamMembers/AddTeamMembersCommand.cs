using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Behaviors;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Domain.Entities;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Teams;

public sealed record AddTeamMembersCommand(
    Guid UserId,
    Guid TeamId,
    List<AddTeamMemberItem> Members,
    CancellationToken CancellationToken
) : IRequest, IRequireTeamRole
{
    Guid IRequireTeamRole.TeamId => TeamId;
    Guid IRequireTeamRole.CurrentUserId => UserId;
    TeamMemberRole[] IRequireTeamRole.AllowedRoles =>
        [TeamMemberRole.Admin, TeamMemberRole.CoAdmin];
}

public sealed record AddTeamMemberItem(Guid MemberId, TeamMemberRole? Role);

public sealed class AddTeamMembersCommandHandler : IRequestHandler<AddTeamMembersCommand>
{
    private readonly IAppDbContext _context;

    public AddTeamMembersCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(AddTeamMembersCommand request, CancellationToken cancellationToken)
    {
        var teamId = request.TeamId;
        var currentUserId = request.UserId;

        // TeamRoleBehavior already validated that current user is Admin or CoAdmin
        // But we still need to check if they're Admin to enforce CoAdmin restrictions
        var currentUserRole = await _context
            .TeamMembers.AsNoTracking()
            .Where(tm => tm.TeamId == teamId && tm.UserId == currentUserId)
            .Select(tm => tm.Role)
            .FirstOrDefaultAsync(cancellationToken);

        var isAdmin = currentUserRole == TeamMemberRole.Admin;

        // Get existing member IDs to avoid duplicates (use HashSet for O(1) lookup)
        var existingMemberIds = await _context
            .TeamMembers.AsNoTracking()
            .Where(tm => tm.TeamId == teamId)
            .Select(tm => tm.UserId)
            .ToListAsync(cancellationToken);

        var existingSet = new HashSet<Guid>(existingMemberIds);
        var now = DateTimeOffset.UtcNow;
        var newMembers = new List<TeamMember>();

        foreach (var item in request.Members)
        {
            // Skip if user is already a member
            if (existingSet.Contains(item.MemberId))
                continue;

            // CoAdmin can only add members with role Member
            var role = item.Role ?? TeamMemberRole.Member;
            if (!isAdmin && role != TeamMemberRole.Member)
            {
                throw new ForbiddenException(ErrorCodes.Forbidden);
            }

            newMembers.Add(
                new TeamMember
                {
                    Id = Guid.NewGuid(),
                    TeamId = teamId,
                    UserId = item.MemberId,
                    Role = role,
                    JoinedAt = now,
                }
            );

            // Add to existing set to prevent duplicate within the same batch request
            existingSet.Add(item.MemberId);
        }

        if (newMembers.Count > 0)
        {
            _context.TeamMembers.AddRange(newMembers);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
