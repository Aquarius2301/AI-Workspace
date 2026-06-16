using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Behavior;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Teams;

public sealed record RemoveTeamMemberCommand(Guid CurrentUserId, Guid TeamId, Guid UserId)
    : IRequest,
        IRequireTeamRole
{
    TeamMemberRole[] IRequireTeamRole.AllowedRoles => [TeamMemberRole.Admin];
}

public sealed class RemoveTeamMemberCommandHandler : IRequestHandler<RemoveTeamMemberCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public RemoveTeamMemberCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RemoveTeamMemberCommand request, CancellationToken cancellationToken)
    {
        var teamMember =
            await _unitOfWork
                .TeamMembers.GetQuery()
                .FirstOrDefaultAsync(
                    tm => tm.TeamId == request.TeamId && tm.UserId == request.UserId,
                    cancellationToken
                )
            ?? throw new NotFoundException("Member not found in this team");

        // Ensure at least one admin remains
        if (teamMember.Role == TeamMemberRole.Admin)
        {
            var adminCount = await _unitOfWork
                .TeamMembers.GetQuery()
                .CountAsync(
                    tm => tm.TeamId == request.TeamId && tm.Role == TeamMemberRole.Admin,
                    cancellationToken
                );

            if (adminCount <= 1)
                throw new BadRequestException("Team must have at least one admin");
        }

        // Remove member from all project memberships in this team
        var projectIds = await _unitOfWork
            .Projects.GetQuery()
            .Where(p => p.TeamId == request.TeamId)
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);

        if (projectIds.Count > 0)
        {
            var projectMembers = await _unitOfWork
                .ProjectMembers.GetQuery()
                .Where(pm => pm.UserId == request.UserId && projectIds.Contains(pm.ProjectId))
                .ToListAsync(cancellationToken);

            _unitOfWork.ProjectMembers.RemoveRange(projectMembers);
        }

        _unitOfWork.TeamMembers.Remove(teamMember);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
