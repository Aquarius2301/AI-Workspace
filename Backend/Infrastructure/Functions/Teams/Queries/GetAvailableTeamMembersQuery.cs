using DataAccess.UnitOfWork;
using Infrastructure.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Teams;

public sealed record GetAvailableTeamMembersQuery(
    Guid CurrentUserId,
    Guid TeamId,
    PaginationRequest Pagination,
    string? Search,
    CancellationToken CancellationToken = default
) : IRequest<PaginationResult<AvailableTeamMemberItem>>;

public sealed record AvailableTeamMemberItem(Guid UserId, string UserName, string Email);

public sealed class GetAvailableTeamMembersQueryHandler
    : IRequestHandler<GetAvailableTeamMembersQuery, PaginationResult<AvailableTeamMemberItem>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAvailableTeamMembersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginationResult<AvailableTeamMemberItem>> Handle(
        GetAvailableTeamMembersQuery request,
        CancellationToken cancellationToken
    )
    {
        var memberUserIds = await _unitOfWork
            .TeamMembers.ReadOnly()
            .Where(tm => tm.TeamId == request.TeamId)
            .Select(tm => tm.UserId)
            .ToListAsync(cancellationToken);

        var query = _unitOfWork.Users.ReadOnly().Where(u => !memberUserIds.Contains(u.Id));

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchLower = request.Search.ToLower();
            query = query.Where(u => u.Name.Contains(searchLower) || u.Email.Contains(searchLower));
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(u => u.Name)
            .Skip((request.Pagination.Page - 1) * request.Pagination.PageSize)
            .Take(request.Pagination.PageSize)
            .Select(u => new AvailableTeamMemberItem(u.Id, u.Name, u.Email))
            .ToListAsync(cancellationToken);

        return new PaginationResult<AvailableTeamMemberItem>(
            request.Pagination.Page,
            request.Pagination.PageSize,
            total,
            items
        );
    }
}
