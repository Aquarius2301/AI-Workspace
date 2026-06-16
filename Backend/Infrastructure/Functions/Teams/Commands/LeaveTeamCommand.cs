using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Teams;

public sealed record LeaveTeamCommand(Guid CurrentUserId, Guid TeamId) : IRequest;

public sealed class LeaveTeamCommandHandler : IRequestHandler<LeaveTeamCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public LeaveTeamCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(LeaveTeamCommand request, CancellationToken cancellationToken)
    {
        var teamMember = await _unitOfWork
            .TeamMembers.GetQuery()
            .FirstOrDefaultAsync(
                tm => tm.TeamId == request.TeamId && tm.UserId == request.CurrentUserId,
                cancellationToken
            );

        if (teamMember is null)
            throw new BadRequestException("You are not a member of this team");

        // Ensure at least one admin remains if leaving admin
        if (teamMember.Role == TeamMemberRole.Admin)
        {
            var adminCount = await _unitOfWork
                .TeamMembers.GetQuery()
                .CountAsync(
                    tm => tm.TeamId == request.TeamId && tm.Role == TeamMemberRole.Admin,
                    cancellationToken
                );

            if (adminCount <= 1)
                throw new BadRequestException(
                    "Team must have at least one admin. Transfer admin role before leaving."
                );
        }

        // Remove user from all project memberships in this team
        var projectIds = await _unitOfWork
            .Projects.GetQuery()
            .Where(p => p.TeamId == request.TeamId)
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);

        if (projectIds.Count > 0)
        {
            var projectMembers = await _unitOfWork
                .ProjectMembers.GetQuery()
                .Where(pm =>
                    pm.UserId == request.CurrentUserId && projectIds.Contains(pm.ProjectId)
                )
                .ToListAsync(cancellationToken);

            _unitOfWork.ProjectMembers.RemoveRange(projectMembers);
        }

        _unitOfWork.TeamMembers.Remove(teamMember);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
