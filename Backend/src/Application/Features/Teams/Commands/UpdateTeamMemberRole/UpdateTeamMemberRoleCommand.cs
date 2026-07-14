using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Behaviors;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Teams;

public sealed record UpdateTeamMemberRoleCommand(
    Guid UserId,
    Guid TeamId,
    Guid MemberId,
    TeamMemberRole? Role,
    CancellationToken CancellationToken
) : IRequest, IRequireTeamRole
{
    Guid IRequireTeamRole.TeamId => TeamId;
    Guid IRequireTeamRole.CurrentUserId => UserId;
    TeamMemberRole[] IRequireTeamRole.AllowedRoles => [TeamMemberRole.Admin];
}

public sealed class UpdateTeamMemberRoleCommandHandler
    : IRequestHandler<UpdateTeamMemberRoleCommand>
{
    private readonly IAppDbContext _context;

    public UpdateTeamMemberRoleCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(
        UpdateTeamMemberRoleCommand request,
        CancellationToken cancellationToken
    )
    {
        var teamMember =
            await _context.TeamMembers.FirstOrDefaultAsync(
                tm => tm.UserId == request.MemberId && tm.TeamId == request.TeamId,
                cancellationToken
            ) ?? throw new NotFoundException(ErrorCodes.UserNotFound);

        if (request.Role is not null)
            teamMember.Role = request.Role.Value;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
