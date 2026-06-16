using DataAccess.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Teams;

public sealed record GetAvailableTeamMembersQuery(Guid CurrentUserId, Guid TeamId)
    : IRequest<List<AvailableTeamMemberItem>>;

public sealed record AvailableTeamMemberItem(Guid UserId, string UserName, string? Email);

public sealed class GetAvailableTeamMembersQueryHandler
    : IRequestHandler<GetAvailableTeamMembersQuery, List<AvailableTeamMemberItem>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAvailableTeamMembersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<AvailableTeamMemberItem>> Handle(
        GetAvailableTeamMembersQuery request,
        CancellationToken cancellationToken
    )
    {
        var memberUserIds = await _unitOfWork
            .TeamMembers.GetQuery()
            .Where(tm => tm.TeamId == request.TeamId)
            .Select(tm => tm.UserId)
            .ToListAsync(cancellationToken);

        var availableMembers = await _unitOfWork
            .Users.GetQuery()
            .Where(u => !memberUserIds.Contains(u.Id))
            .Select(u => new AvailableTeamMemberItem(u.Id, u.Name, u.Email))
            .ToListAsync(cancellationToken);

        return availableMembers;
    }
}
