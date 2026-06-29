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
    List<AddTeamMemberRequest> Members,
    CancellationToken CancellationToken = default
) : IRequest, IRequireTeamRole
{
    TeamMemberRole[] IRequireTeamRole.AllowedRoles =>
        [TeamMemberRole.Admin, TeamMemberRole.CoAdmin];
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
        var team =
            await _unitOfWork
                .Teams.GetQuery()
                .FirstOrDefaultAsync(t => t.Id == request.TeamId, cancellationToken)
            ?? throw new NotFoundException(ErrorCodes.TeamNotFound);

        // Get the current user's role to enforce permissions
        var currentMember = await _unitOfWork
            .TeamMembers.GetQuery()
            .FirstOrDefaultAsync(
                tm => tm.TeamId == request.TeamId && tm.UserId == request.CurrentUserId,
                cancellationToken
            );

        var isAdmin = currentMember?.Role == TeamMemberRole.Admin;

        foreach (var memberReq in request.Members)
        {
            TeamMemberRole role;
            if (string.IsNullOrWhiteSpace(memberReq.Role))
            {
                role = TeamMemberRole.Member;
            }
            else if (!Enum.TryParse<TeamMemberRole>(memberReq.Role, true, out role))
            {
                throw new BadRequestException(ErrorCodes.InvalidRoleRequest);
            }

            // Admin and CoAdmin cannot add members with Admin role
            if (role == TeamMemberRole.Admin)
                throw new BadRequestException(ErrorCodes.NoPermissionAddMemberRole);

            // CoAdmin cannot add members with CoAdmin role
            if (!isAdmin && role == TeamMemberRole.CoAdmin)
                throw new BadRequestException(ErrorCodes.NoPermissionAddMemberRole);

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
                JoinedAt = DateTimeOffset.UtcNow,
            };

            _unitOfWork.TeamMembers.Add(teamMember);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
