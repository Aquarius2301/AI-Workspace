using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Behavior;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Teams;

public sealed record AddTeamMemberRequest(Guid UserId, string? Role);

public sealed record AddTeamMembersCommand(
    Guid CurrentUserId,
    Guid TeamId,
    List<AddTeamMemberRequest> Members
) : IRequest, IRequireTeamRole
{
    TeamMemberRole[] IRequireTeamRole.AllowedRoles => [TeamMemberRole.Admin];
}

public sealed class AddTeamMembersCommandHandler : IRequestHandler<AddTeamMembersCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddTeamMembersCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AddTeamMembersCommand request, CancellationToken cancellationToken)
    {
        var team = await _unitOfWork
            .Teams.GetQuery()
            .FirstOrDefaultAsync(t => t.Id == request.TeamId, cancellationToken);

        if (team is null)
            throw new NotFoundException("Team not found");

        foreach (var memberReq in request.Members)
        {
            TeamMemberRole role;
            if (string.IsNullOrWhiteSpace(memberReq.Role))
            {
                role = TeamMemberRole.Member;
            }
            else if (!Enum.TryParse<TeamMemberRole>(memberReq.Role, true, out role))
            {
                throw new BadRequestException(
                    $"Invalid role '{memberReq.Role}'. Valid values: Admin, Leader, Member"
                );
            }

            // Check if already a member
            var existing = await _unitOfWork
                .TeamMembers.GetQuery()
                .AnyAsync(
                    tm => tm.TeamId == request.TeamId && tm.UserId == memberReq.UserId,
                    cancellationToken
                );

            if (existing)
                continue;

            var user = await _unitOfWork
                .Users.GetQuery()
                .FirstOrDefaultAsync(u => u.Id == memberReq.UserId, cancellationToken);

            if (user is null)
                continue;

            var teamMember = new BusinessObject.Entities.TeamMember
            {
                Id = Guid.NewGuid(),
                TeamId = request.TeamId,
                UserId = memberReq.UserId,
                Role = role,
                JoinedAt = DateTime.UtcNow,
            };

            _unitOfWork.TeamMembers.Add(teamMember);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
