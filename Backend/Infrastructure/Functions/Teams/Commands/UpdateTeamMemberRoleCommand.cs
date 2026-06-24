using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Behavior;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Teams;

public sealed record UpdateTeamMemberRoleCommand(
    Guid CurrentUserId,
    Guid TeamId,
    Guid UserId,
    string? Role
) : IRequest, IRequireTeamRole
{
    TeamMemberRole[] IRequireTeamRole.AllowedRoles => [TeamMemberRole.Admin];
}

public sealed class UpdateTeamMemberRoleCommandHandler
    : IRequestHandler<UpdateTeamMemberRoleCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTeamMemberRoleCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        UpdateTeamMemberRoleCommand request,
        CancellationToken cancellationToken
    )
    {
        var teamMember =
            await _unitOfWork
                .TeamMembers.GetQuery()
                .Include(tm => tm.User)
                .FirstOrDefaultAsync(
                    tm => tm.TeamId == request.TeamId && tm.UserId == request.UserId,
                    cancellationToken
                )
            ?? throw new NotFoundException(ErrorCodes.MemberNotFound);

        // Parse role
        TeamMemberRole role;
        if (string.IsNullOrWhiteSpace(request.Role))
        {
            role = TeamMemberRole.Member;
        }
        else if (!Enum.TryParse(request.Role, true, out role))
        {
            throw new BadRequestException(ErrorCodes.InvalidRoleRequest);
        }

        // Ensure at least one admin remains
        if (teamMember.Role == TeamMemberRole.Admin && role != TeamMemberRole.Admin)
        {
            var adminCount = await _unitOfWork
                .TeamMembers.GetQuery()
                .CountAsync(
                    tm => tm.TeamId == request.TeamId && tm.Role == TeamMemberRole.Admin,
                    cancellationToken
                );

            if (adminCount <= 1)
                throw new BadRequestException(ErrorCodes.TeamMinOneAdmin);
        }

        teamMember.Role = role;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
