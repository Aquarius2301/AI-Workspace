using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Teams;

public sealed record GetTeamMemberQuery(
    Guid TeamId,
    Guid MemberId,
    CancellationToken CancellationToken = default
) : IRequest<TeamMemberItem>;

public sealed class GetTeamMemberQueryHandler : IRequestHandler<GetTeamMemberQuery, TeamMemberItem>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTeamMemberQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TeamMemberItem> Handle(
        GetTeamMemberQuery request,
        CancellationToken cancellationToken
    )
    {
        var member =
            await _unitOfWork
                .TeamMembers.ReadOnly()
                .Where(tm => tm.TeamId == request.TeamId && tm.UserId == request.MemberId)
                .Select(tm => new TeamMemberItem(
                    tm.UserId,
                    tm.User.Name,
                    tm.User.AvatarUrl ?? "",
                    tm.Role.ToString(),
                    tm.JoinedAt,
                    tm.User.Email,
                    tm.User.LastActiveAt
                ))
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(ErrorCodes.MemberNotFound);

        return member;
    }
}
