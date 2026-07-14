using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Behaviors;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Teams;

public sealed record UpdateTeamCommand(
    Guid UserId,
    Guid TeamId,
    string? Name,
    string? Description,
    CancellationToken CancellationToken
) : IRequest, IRequireTeamRole
{
    Guid IRequireTeamRole.TeamId => TeamId;
    Guid IRequireTeamRole.CurrentUserId => UserId;
    TeamMemberRole[] IRequireTeamRole.AllowedRoles => [TeamMemberRole.Admin];
}

public sealed class UpdateTeamCommandHandler : IRequestHandler<UpdateTeamCommand>
{
    private readonly IAppDbContext _context;

    public UpdateTeamCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateTeamCommand request, CancellationToken cancellationToken)
    {
        var team =
            await _context.Teams.FirstOrDefaultAsync(t => t.Id == request.TeamId, cancellationToken)
            ?? throw new NotFoundException(ErrorCodes.NotFound);

        if (request.Name is not null)
            team.Name = request.Name;

        if (request.Description is not null)
            team.Description = request.Description;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
