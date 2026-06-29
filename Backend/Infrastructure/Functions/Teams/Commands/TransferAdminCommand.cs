using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Behavior;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Teams;

/// <summary>
/// Transfers the Admin role from the current Admin to a CoAdmin.
/// Only the sole Admin can call this. After transfer:
/// - The current Admin becomes CoAdmin
/// - The target CoAdmin becomes Admin
/// </summary>
public sealed record TransferAdminCommand(
    Guid CurrentUserId,
    Guid TeamId,
    Guid TargetUserId,
    CancellationToken CancellationToken = default
) : IRequest, IRequireTeamRole
{
    TeamMemberRole[] IRequireTeamRole.AllowedRoles => [TeamMemberRole.Admin];
}

public sealed class TransferAdminCommandHandler : IRequestHandler<TransferAdminCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public TransferAdminCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(TransferAdminCommand request, CancellationToken cancellationToken)
    {
        // Get current Admin's membership
        var currentAdmin = await _unitOfWork
            .TeamMembers.GetQuery()
            .FirstOrDefaultAsync(
                tm => tm.TeamId == request.TeamId && tm.UserId == request.CurrentUserId,
                cancellationToken
            )
            ?? throw new NotFoundException(ErrorCodes.MemberNotFound);

        if (currentAdmin.Role != TeamMemberRole.Admin)
            throw new ForbiddenException(ErrorCodes.NoPermissionManageTeam);

        // Ensure there is only one Admin (the caller)
        var adminCount = await _unitOfWork
            .TeamMembers.GetQuery()
            .CountAsync(
                tm => tm.TeamId == request.TeamId && tm.Role == TeamMemberRole.Admin,
                cancellationToken
            );

        if (adminCount != 1)
            throw new BadRequestException(ErrorCodes.TeamMinOneAdminTransferRole);

        // Get target member (must be a CoAdmin)
        var targetMember = await _unitOfWork
            .TeamMembers.GetQuery()
            .FirstOrDefaultAsync(
                tm => tm.TeamId == request.TeamId && tm.UserId == request.TargetUserId,
                cancellationToken
            )
            ?? throw new NotFoundException(ErrorCodes.MemberNotFound);

        if (targetMember.Role != TeamMemberRole.CoAdmin)
            throw new BadRequestException(ErrorCodes.InvalidRoleRequest);

        // Swap roles
        currentAdmin.Role = TeamMemberRole.CoAdmin;
        targetMember.Role = TeamMemberRole.Admin;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
