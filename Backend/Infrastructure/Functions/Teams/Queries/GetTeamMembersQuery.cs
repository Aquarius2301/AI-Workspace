using DataAccess.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Teams;

public sealed record GetTeamMembersQuery(Guid TeamId) : IRequest<List<TeamMemberItem>>;

public sealed record TeamMemberItem(
    Guid UserId,
    string UserName,
    string? Role,
    DateTime JoinedAt,
    string Email,
    DateTime? LastActiveAt
);

public sealed class GetTeamMembersQueryHandler
    : IRequestHandler<GetTeamMembersQuery, List<TeamMemberItem>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTeamMembersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<TeamMemberItem>> Handle(
        GetTeamMembersQuery request,
        CancellationToken cancellationToken
    )
    {
        var members = await _unitOfWork
            .TeamMembers.ReadOnly()
            .Where(tm => tm.TeamId == request.TeamId)
            .Select(tm => new TeamMemberItem(
                tm.UserId,
                tm.User.Name,
                tm.Role.ToString(),
                tm.JoinedAt,
                tm.User.Email,
                tm.User.LastActiveAt
            ))
            .ToListAsync(cancellationToken);

        return members;
    }
}
