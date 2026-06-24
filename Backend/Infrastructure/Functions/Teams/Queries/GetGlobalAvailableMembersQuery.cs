using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Teams;

public sealed record GetGlobalAvailableMembersQuery(Guid CurrentUserId)
    : IRequest<List<AvailableTeamMemberItem>>;

public sealed class GetGlobalAvailableMembersQueryHandler
    : IRequestHandler<GetGlobalAvailableMembersQuery, List<AvailableTeamMemberItem>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetGlobalAvailableMembersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<AvailableTeamMemberItem>> Handle(
        GetGlobalAvailableMembersQuery request,
        CancellationToken cancellationToken
    )
    {
        // Verify the current user is an Admin in at least one team
        var isAdmin = await _unitOfWork
            .TeamMembers.GetQuery()
            .AnyAsync(
                tm => tm.UserId == request.CurrentUserId && tm.Role == TeamMemberRole.Admin,
                cancellationToken
            );

        if (!isAdmin)
            throw new ForbiddenException(ErrorCodes.AdminOnlyViewMembers);

        // Get all users who are not members of any team
        var memberUserIds = await _unitOfWork
            .TeamMembers.GetQuery()
            .Select(tm => tm.UserId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var availableMembers = await _unitOfWork
            .Users.GetQuery()
            .Where(u => !memberUserIds.Contains(u.Id))
            .Select(u => new AvailableTeamMemberItem(u.Id, u.Name, u.Email))
            .ToListAsync(cancellationToken);

        return availableMembers;
    }
}
