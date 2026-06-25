using DataAccess.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Teams;

public sealed record GetTeamQuery(Guid TeamId, CancellationToken CancellationToken = default)
    : IRequest<TeamDetail?>;

public sealed record TeamDetail(Guid Id, string Name, string? Description);

public sealed class GetTeamQueryHandler : IRequestHandler<GetTeamQuery, TeamDetail?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTeamQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TeamDetail?> Handle(GetTeamQuery request, CancellationToken cancellationToken)
    {
        var team = await _unitOfWork
            .Teams.ReadOnly()
            .Where(t => t.Id == request.TeamId)
            .Select(t => new TeamDetail(t.Id, t.Name, t.Description))
            .FirstOrDefaultAsync(cancellationToken);

        return team;
    }
}
