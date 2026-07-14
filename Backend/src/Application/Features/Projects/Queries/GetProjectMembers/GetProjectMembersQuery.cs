using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Application.Common.Models;
using AIWorkspace.Application.Helpers;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Projects;

public sealed record GetProjectMembersQuery(
    Guid CurrentUserId,
    Guid ProjectId,
    PaginationRequest Pagination,
    string? Search,
    ProjectRole? Role,
    CancellationToken CancellationToken
) : IRequest<PaginationResult<ProjectMemberItem>>;

public sealed record ProjectMemberItem(
    Guid UserId,
    string UserName,
    string UserEmail,
    string Role,
    DateTimeOffset JoinedAt
);

public sealed class GetProjectMembersQueryHandler
    : IRequestHandler<GetProjectMembersQuery, PaginationResult<ProjectMemberItem>>
{
    private readonly IAppDbContext _context;

    public GetProjectMembersQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginationResult<ProjectMemberItem>> Handle(
        GetProjectMembersQuery request,
        CancellationToken cancellationToken
    )
    {
        var projectId = request.ProjectId;
        var currentUserId = request.CurrentUserId;

        // Verify the requesting user has access to the project
        // They must either be a team Admin/CoAdmin or a project member
        var project =
            await _context
                .Projects.AsNoTracking()
                .Where(p => p.Id == projectId)
                .Select(p => new { p.TeamId })
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(ErrorCodes.ProjectNotFound);

        // Check team-level access (Admin or CoAdmin)
        var isTeamAdminOrCoAdmin = await _context
            .TeamMembers.AsNoTracking()
            .AnyAsync(
                tm =>
                    tm.TeamId == project.TeamId
                    && tm.UserId == currentUserId
                    && (tm.Role == TeamMemberRole.Admin || tm.Role == TeamMemberRole.CoAdmin),
                cancellationToken
            );

        // Check project-level membership
        var isProjectMember = await _context
            .ProjectMembers.AsNoTracking()
            .AnyAsync(
                pm => pm.ProjectId == projectId && pm.UserId == currentUserId,
                cancellationToken
            );

        // If neither, deny access
        if (!isTeamAdminOrCoAdmin && !isProjectMember)
        {
            throw new NotFoundException(ErrorCodes.NotFound);
        }

        // Build query for project members with user data
        var query = _context
            .ProjectMembers.AsNoTracking()
            .Where(pm => pm.ProjectId == projectId)
            .Include(pm => pm.User)
            .AsQueryable();

        // Apply search filter on user name
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search;
            query = query.Where(pm =>
                EF.Functions.Collate(pm.User.Name, "SQL_Latin1_General_CP1_CI_AI").Contains(search)
                || EF.Functions.Collate(pm.User.Email, "SQL_Latin1_General_CP1_CI_AI")
                    .Contains(search)
            );
        }

        // Apply role filter if provided
        if (request.Role.HasValue)
        {
            query = query.Where(pm => pm.Role == request.Role.Value);
        }

        // Get total count before pagination
        var total = await query.CountAsync(cancellationToken);

        // Apply pagination
        var members = await query
            .OrderBy(pm => pm.JoinedAt)
            .Skip((request.Pagination.Page - 1) * request.Pagination.PageSize)
            .Take(request.Pagination.PageSize)
            .ToListAsync(cancellationToken);

        // Map to result items
        var items = members
            .Select(pm => new ProjectMemberItem(
                pm.UserId,
                pm.User.Name,
                pm.User.Email,
                pm.Role.ToString(),
                pm.JoinedAt
            ))
            .ToList();

        return new PaginationResult<ProjectMemberItem>(
            request.Pagination.Page,
            request.Pagination.PageSize,
            total,
            items
        );
    }
}
