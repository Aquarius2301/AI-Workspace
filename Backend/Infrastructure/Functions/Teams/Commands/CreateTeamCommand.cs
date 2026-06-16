using BusinessObject.Entities;
using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using MediatR;

namespace Infrastructure.Functions.Teams;

public sealed record CreateTeamCommand(Guid CurrentUserId, string Name, string? Description)
    : IRequest<TeamDetail>;

public sealed class CreateTeamCommandHandler : IRequestHandler<CreateTeamCommand, TeamDetail>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateTeamCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TeamDetail> Handle(
        CreateTeamCommand request,
        CancellationToken cancellationToken
    )
    {
        var team = new Team
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
        };

        var teamMember = new TeamMember
        {
            Id = Guid.NewGuid(),
            TeamId = team.Id,
            UserId = request.CurrentUserId,
            Role = TeamMemberRole.Admin,
            JoinedAt = DateTime.UtcNow,
        };

        _unitOfWork.Teams.Add(team);
        _unitOfWork.TeamMembers.Add(teamMember);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new TeamDetail(team.Id, team.Name, team.Description);
    }
}
