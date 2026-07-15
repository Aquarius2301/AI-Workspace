using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Teams;

public sealed record RemoveTeamMemberCommand(
    Guid UserId,
    Guid TeamId,
    Guid MemberId,
    CancellationToken CancellationToken
) : IRequest;

public sealed class RemoveTeamMemberCommandHandler : IRequestHandler<RemoveTeamMemberCommand>
{
    private readonly IAppDbContext _context;

    public RemoveTeamMemberCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(RemoveTeamMemberCommand request, CancellationToken cancellationToken)
    {
        // ── Fetch target team member ──
        var targetMember =
            await _context.TeamMembers.FirstOrDefaultAsync(
                tm => tm.TeamId == request.TeamId && tm.UserId == request.MemberId,
                cancellationToken
            ) ?? throw new NotFoundException(ErrorCodes.UserNotFound);

        // ── Fetch current user's role ──
        var currentUserRole = await _context
            .TeamMembers.AsNoTracking()
            .Where(tm => tm.TeamId == request.TeamId && tm.UserId == request.UserId)
            .Select(tm => tm.Role)
            .FirstOrDefaultAsync(cancellationToken);

        // ── Validation ──
        // Cannot remove yourself
        if (request.UserId == request.MemberId)
        {
            throw new BadRequestException(ErrorCodes.BadRequest);
        }

        // Check permissions based on current user's role
        switch (currentUserRole)
        {
            case TeamMemberRole.Admin:
                // Admin can remove Member or CoAdmin but not another Admin
                if (targetMember.Role == TeamMemberRole.Admin)
                {
                    throw new ForbiddenException(ErrorCodes.Forbidden);
                }
                break;

            case TeamMemberRole.CoAdmin:
                // CoAdmin can only remove Member
                if (targetMember.Role != TeamMemberRole.Member)
                {
                    throw new ForbiddenException(ErrorCodes.Forbidden);
                }
                break;

            default:
                // Member cannot remove anyone
                throw new ForbiddenException(ErrorCodes.Forbidden);
        }

        // ── Remove team member ──
        _context.TeamMembers.Remove(targetMember);

        // ── Get all project IDs belonging to this team ──
        var teamProjectIds = await _context
            .Projects.AsNoTracking()
            .Where(p => p.TeamId == request.TeamId)
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);

        // ── Unassign tasks assigned to this user in team's projects ──
        await _context
            .TaskItems
            .Where(t => teamProjectIds.Contains(t.ProjectId) && t.AssignedToId == request.MemberId)
            .ExecuteUpdateAsync(
                t => t.SetProperty(x => x.AssignedToId, (Guid?)null),
                cancellationToken
            );

        // ── Remove project memberships for this user in team's projects ──
        await _context
            .ProjectMembers
            .Where(pm => teamProjectIds.Contains(pm.ProjectId) && pm.UserId == request.MemberId)
            .ExecuteDeleteAsync(cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
