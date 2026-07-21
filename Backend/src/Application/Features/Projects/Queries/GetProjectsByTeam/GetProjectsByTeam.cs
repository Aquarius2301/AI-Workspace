using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Application.Common.Models;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Projects;

public sealed record GetProjectsByTeamQuery(
    Guid CurrentUserId,
    Guid TeamId,
    string? Search,
    ProjectVisibility? Visibility,
    PaginationRequest Pagination,
    CancellationToken CancellationToken = default
) : IRequest<PaginationResult<ProjectItem>>;

public sealed record ProjectItem(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    string CreatorName,
    string Visibility,
    bool CanView,
    int MemberCount,
    int CompletedTaskCount,
    int TotalTaskCount
);

public sealed class GetProjectsByTeamQueryHandler
    : IRequestHandler<GetProjectsByTeamQuery, PaginationResult<ProjectItem>>
{
    private readonly IAppDbContext _context;

    public GetProjectsByTeamQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginationResult<ProjectItem>> Handle(
        GetProjectsByTeamQuery request,
        CancellationToken cancellationToken
    )
    {
        var currentUserId = request.CurrentUserId;
        var teamId = request.TeamId;
        var search = request.Search;
        var visibility = request.Visibility;
        var pagination = request.Pagination;

        // Verify the current user is a member of this team and get their role
        var teamMembership = await _context
            .TeamMembers.AsNoTracking()
            .FirstOrDefaultAsync(
                tm => tm.TeamId == teamId && tm.UserId == currentUserId,
                cancellationToken
            );

        if (teamMembership is null)
        {
            throw new NotFoundException(ErrorCodes.TeamNotFound);
        }

        var isTeamAdminOrCoAdmin =
            teamMembership.Role is TeamMemberRole.Admin or TeamMemberRole.CoAdmin;

        // Build query at DB level — no client-side evaluation
        var query = _context.Projects.AsNoTracking().Where(p => p.TeamId == teamId);

        // Non-admin users can only see public projects or projects they belong to
        if (!isTeamAdminOrCoAdmin)
        {
            query = query.Where(p =>
                p.Visibility == ProjectVisibility.Public
                || p.ProjectMembers.Any(pm => pm.UserId == currentUserId)
            );
        }

        // Apply search filter at DB level using SQL Server collation
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p =>
                EF.Functions.Collate(p.Name, "SQL_Latin1_General_CP1_CI_AI").Contains(search)
                || EF.Functions.Collate(p.Description ?? "", "SQL_Latin1_General_CP1_CI_AI")
                    .Contains(search)
            );
        }

        // Apply visibility filter at DB level if provided
        if (visibility.HasValue)
        {
            query = query.Where(p => p.Visibility == visibility.Value);
        }

        // Get total count at DB level
        var total = await query.CountAsync(cancellationToken);

        // Apply pagination and projection at DB level — single round-trip
        var items = await query
            .OrderBy(p => p.Name)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(p => new ProjectItem(
                p.Id,
                p.Name,
                p.Slug,
                p.Description,
                p.Creator.Name,
                p.Visibility.ToString(),
                isTeamAdminOrCoAdmin || p.ProjectMembers.Any(pm => pm.UserId == currentUserId),
                p.ProjectMembers.Count,
                p.TaskItems.Count(t => t.Status == TaskItemStatus.Done),
                p.TaskItems.Count
            ))
            .ToListAsync(cancellationToken);

        return new PaginationResult<ProjectItem>(
            pagination.Page,
            pagination.PageSize,
            total,
            items
        );
    }
}
