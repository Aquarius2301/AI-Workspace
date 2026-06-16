using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Behavior;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Teams;

public sealed record UpdateTeamCommand(
    Guid CurrentUserId,
    Guid TeamId,
    string? Name,
    string? Description
) : IRequest<TeamDetail>, IRequireTeamRole
{
    TeamMemberRole[] IRequireTeamRole.AllowedRoles => [TeamMemberRole.Admin];
}

public sealed class UpdateTeamCommandHandler : IRequestHandler<UpdateTeamCommand, TeamDetail>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTeamCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TeamDetail> Handle(
        UpdateTeamCommand request,
        CancellationToken cancellationToken
    )
    {
        var team =
            await _unitOfWork
                .Teams.GetQuery()
                .FirstOrDefaultAsync(t => t.Id == request.TeamId, cancellationToken)
            ?? throw new NotFoundException("Team not found");

        if (request.Name is not null)
            team.Name = request.Name;

        if (request.Description is not null)
            team.Description = request.Description;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new TeamDetail(team.Id, team.Name, team.Description);
    }
}
