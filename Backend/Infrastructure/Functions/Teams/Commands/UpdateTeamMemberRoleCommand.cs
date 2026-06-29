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
    string? Role,
    CancellationToken CancellationToken = default
) : IRequest, IRequireTeamRole
{
    TeamMemberRole[] IRequireTeamRole.AllowedRoles =>
        [TeamMemberRole.Admin, TeamMemberRole.CoAdmin];
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

        // Get the current user's role
        var currentMember = await _unitOfWork
            .TeamMembers.GetQuery()
            .FirstOrDefaultAsync(
                tm => tm.TeamId == request.TeamId && tm.UserId == request.CurrentUserId,
                cancellationToken
            );

        var isAdmin = currentMember?.Role == TeamMemberRole.Admin;

        // CoAdmin cannot modify roles of Admin or other CoAdmin (same rank or higher)
        if (
            !isAdmin
            && (
                teamMember.Role == TeamMemberRole.Admin || teamMember.Role == TeamMemberRole.CoAdmin
            )
        )
        {
            throw new ForbiddenException(ErrorCodes.NoPermissionUpdateMemberRole);
        }

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

        // Admin and CoAdmin cannot assign Admin role to anyone
        if (role == TeamMemberRole.Admin)
            throw new ForbiddenException(ErrorCodes.NoPermissionUpdateMemberRole);

        // CoAdmin cannot assign CoAdmin role to anyone
        if (!isAdmin && role == TeamMemberRole.CoAdmin)
            throw new ForbiddenException(ErrorCodes.NoPermissionUpdateMemberRole);

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
