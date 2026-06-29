using DataAccess.UnitOfWork;
using Infrastructure.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Teams;

public sealed record GetTeamsQuery(
    Guid CurrentUserId,
    bool MyTeams,
    string? Search,
    PaginationRequest Pagination,
    CancellationToken CancellationToken = default
) : IRequest<PaginationResult<TeamItem>>;

public sealed record TeamItem(
    Guid Id,
    string Name,
    string? Description,
    int MemberCount,
    string? CurrentUserRole
);

public sealed class GetTeamsQueryHandler
    : IRequestHandler<GetTeamsQuery, PaginationResult<TeamItem>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTeamsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginationResult<TeamItem>> Handle(
        GetTeamsQuery request,
        CancellationToken cancellationToken
    )
    {
        var query = _unitOfWork.Teams.ReadOnly();

        if (request.MyTeams)
        {
            var userTeamIds = _unitOfWork
                .TeamMembers.ReadOnly()
                .Where(tm => tm.UserId == request.CurrentUserId)
                .Select(tm => tm.TeamId);

            query = query.Where(t => userTeamIds.Contains(t.Id));
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(t => t.Name.ToLower().Contains(search));
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .Include(t => t.TeamMembers)
            .OrderBy(t => t.Name)
            .Skip((request.Pagination.Page - 1) * request.Pagination.PageSize)
            .Take(request.Pagination.PageSize)
            .ToListAsync(cancellationToken);

        var resultItems = items
            .Select(t => new TeamItem(
                t.Id,
                t.Name,
                t.Description,
                t.TeamMembers.Count,
                t.TeamMembers.FirstOrDefault(tm => tm.UserId == request.CurrentUserId)
                    ?.Role.ToString()
            ))
            .ToList();

        return new PaginationResult<TeamItem>(
            request.Pagination.Page,
            request.Pagination.PageSize,
            total,
            resultItems
        );
    }
}
