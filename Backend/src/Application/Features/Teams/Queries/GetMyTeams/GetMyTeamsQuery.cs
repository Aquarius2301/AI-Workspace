using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Models;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Teams;

public sealed record GetMyTeamsQuery(
    Guid UserId,
    string? Search,
    PaginationRequest Pagination,
    CancellationToken CancellationToken
) : IRequest<PaginationResult<GetMyTeamsResult>>;

public sealed record GetMyTeamsResult(
    Guid Id,
    string Name,
    string? Description,
    string Slug,
    int MemberCount,
    TeamMemberRole CurrentUserRole
);

public sealed class GetMyTeamsQueryHandler
    : IRequestHandler<GetMyTeamsQuery, PaginationResult<GetMyTeamsResult>>
{
    private readonly IAppDbContext _context;

    public GetMyTeamsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginationResult<GetMyTeamsResult>> Handle(
        GetMyTeamsQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = request.UserId;
        var search = request.Search;
        var pagination = request.Pagination;

        // Build query at DB level — start from TeamMembers and join to Teams
        var query = _context.TeamMembers.AsNoTracking().Where(tm => tm.UserId == userId);

        // Apply search filter at DB level using SQL Server collation
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(tm =>
                EF.Functions.Collate(tm.Team.Name, "SQL_Latin1_General_CP1_CI_AI").Contains(search)
                || EF.Functions.Collate(tm.Team.Description ?? "", "SQL_Latin1_General_CP1_CI_AI")
                    .Contains(search)
            );
        }

        // Get total count at DB level
        var total = await query.CountAsync(cancellationToken);

        // Apply pagination and projection at DB level — single round-trip
        var items = await query
            .OrderBy(tm => tm.Team.Name)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(tm => new GetMyTeamsResult(
                tm.Team.Id,
                tm.Team.Name,
                tm.Team.Description,
                tm.Team.Slug,
                tm.Team.TeamMembers.Count,
                tm.Role
            ))
            .ToListAsync(cancellationToken);

        return new PaginationResult<GetMyTeamsResult>(
            pagination.Page,
            pagination.PageSize,
            total,
            items
        );
    }
}
