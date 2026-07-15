using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Models;
using AIWorkspace.Application.Helpers;
using AIWorkspace.Domain.Entities;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Projects;

public sealed record GetMyProjectsQuery(
    Guid CurrentUserId,
    string? Search,
    ProjectVisibility? Visibility,
    PaginationRequest Pagination,
    CancellationToken CancellationToken
) : IRequest<PaginationResult<MyProjectItem>>;

public sealed record MyProjectItem(
    Guid Id,
    string Name,
    string? Description,
    string Slug,
    string TeamName,
    Guid TeamId,
    string Visibility,
    string UserRole,
    int MemberCount,
    int CompletedTaskCount,
    int TotalTaskCount
);

public sealed class GetMyProjectsQueryHandler
    : IRequestHandler<GetMyProjectsQuery, PaginationResult<MyProjectItem>>
{
    private readonly IAppDbContext _context;

    public GetMyProjectsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginationResult<MyProjectItem>> Handle(
        GetMyProjectsQuery request,
        CancellationToken cancellationToken
    )
    {
        // Build query at DB level - only join necessary tables and select what's needed
        var query = _context
            .ProjectMembers.AsNoTracking()
            .Where(pm => pm.UserId == request.CurrentUserId)
            .Include(pm => pm.Project)
                .ThenInclude(p => p.Team)
            .AsQueryable();

        // Apply visibility filter at DB level
        if (request.Visibility.HasValue)
        {
            query = query.Where(pm => pm.Project.Visibility == request.Visibility.Value);
        }

        // Apply search filter at DB level using SQL collation for Vietnamese diacritics
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search;
            query = query.Where(pm =>
                EF.Functions.Collate(pm.Project.Name, "SQL_Latin1_General_CP1_CI_AI")
                    .Contains(search)
                || (
                    pm.Project.Description != null
                    && EF.Functions.Collate(pm.Project.Description, "SQL_Latin1_General_CP1_CI_AI")
                        .Contains(search)
                )
            );
        }

        // Get total count before pagination
        var total = await query.CountAsync(cancellationToken);

        // Apply pagination and select only needed fields
        var items = await query
            .OrderBy(pm => pm.Project.Name)
            .Skip((request.Pagination.Page - 1) * request.Pagination.PageSize)
            .Take(request.Pagination.PageSize)
            .Select(pm => new MyProjectItem(
                pm.ProjectId,
                pm.Project.Name,
                pm.Project.Description,
                pm.Project.Slug,
                pm.Project.Team.Name,
                pm.Project.TeamId,
                pm.Project.Visibility.ToString(),
                pm.Role.ToString(),
                pm.Project.ProjectMembers.Count,
                pm.Project.TaskItems.Count(t => t.Status == TaskItemStatus.Done),
                pm.Project.TaskItems.Count
            ))
            .ToListAsync(cancellationToken);

        return new PaginationResult<MyProjectItem>(
            request.Pagination.Page,
            request.Pagination.PageSize,
            total,
            items
        );
    }
}
