using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Behavior;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Projects.Queries;

public sealed record GetTeamAllProjectsQuery(Guid CurrentUserId, Guid TeamId)
    : IRequest<List<TeamProjectItem>>,
        IRequireTeamRole
{
    TeamMemberRole[] IRequireTeamRole.AllowedRoles => [TeamMemberRole.Admin];
    Guid IRequireTeamRole.TeamId => TeamId;
    Guid IRequireTeamRole.CurrentUserId => CurrentUserId;
}

public sealed class GetTeamAllProjectsQueryHandler
    : IRequestHandler<GetTeamAllProjectsQuery, List<TeamProjectItem>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTeamAllProjectsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<TeamProjectItem>> Handle(
        GetTeamAllProjectsQuery request,
        CancellationToken cancellationToken
    )
    {
        return await _unitOfWork
            .Projects.GetQuery()
            .Where(p => p.TeamId == request.TeamId)
            .Select(p => new TeamProjectItem(
                p.Id,
                p.CreatorId,
                p.Name,
                p.Description,
                p.Visibility.ToString()
            ))
            .ToListAsync(cancellationToken);
    }
}
