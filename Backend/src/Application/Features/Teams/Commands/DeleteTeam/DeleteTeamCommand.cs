using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Behaviors;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Teams;

public sealed record DeleteTeamCommand(
    Guid UserId,
    Guid TeamId,
    CancellationToken CancellationToken
) : IRequest, IRequireTeamRole
{
    Guid IRequireTeamRole.TeamId => TeamId;
    Guid IRequireTeamRole.CurrentUserId => UserId;
    TeamMemberRole[] IRequireTeamRole.AllowedRoles => [TeamMemberRole.Admin];
}

public sealed class DeleteTeamCommandHandler : IRequestHandler<DeleteTeamCommand>
{
    private readonly IAppDbContext _context;

    public DeleteTeamCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteTeamCommand request, CancellationToken cancellationToken)
    {
        var team = await _context.Teams.FirstAsync(t => t.Id == request.TeamId, cancellationToken);

        _context.Teams.Remove(team);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
