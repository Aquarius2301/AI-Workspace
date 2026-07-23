using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Application.Common.Models;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Projects;

public sealed record GetAvailableProjectMembersQuery(
    Guid UserId,
    Guid ProjectId,
    PaginationRequest Pagination,
    string? Search,
    CancellationToken CancellationToken
) : IRequest<PaginationResult<GetAvailableProjectMembersResult>>;

public sealed record GetAvailableProjectMembersResult(
    Guid Id,
    string Name,
    string Email,
    string? AvatarUrl
);

public sealed class GetAvailableProjectMembersQueryHandler
    : IRequestHandler<
        GetAvailableProjectMembersQuery,
        PaginationResult<GetAvailableProjectMembersResult>
    >
{
    private readonly IAppDbContext _context;

    public GetAvailableProjectMembersQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginationResult<GetAvailableProjectMembersResult>> Handle(
        GetAvailableProjectMembersQuery request,
        CancellationToken cancellationToken
    )
    {
        var currentUserId = request.UserId;
        var projectId = request.ProjectId;
        var search = request.Search;
        var pagination = request.Pagination;

        // Get the team ID for this project
        var project = await _context
            .Projects.AsNoTracking()
            .Where(p => p.Id == projectId)
            .Select(p => new { p.TeamId, p.CreatorId })
            .FirstOrDefaultAsync(cancellationToken);

        if (project is null)
            throw new NotFoundException(ErrorCodes.ProjectNotFound);

        // Authorization: current user must be Admin or CoAdmin of the team
        var isAdminOrCoAdmin = await _context
            .TeamMembers.AsNoTracking()
            .AnyAsync(
                tm =>
                    tm.TeamId == project.TeamId
                    && tm.UserId == currentUserId
                    && (tm.Role == TeamMemberRole.Admin || tm.Role == TeamMemberRole.CoAdmin),
                cancellationToken
            );

        if (!isAdminOrCoAdmin)
            throw new ForbiddenException(ErrorCodes.Forbidden);

        // Get team member IDs (potential project members)
        var teamMemberIds = await _context
            .TeamMembers.AsNoTracking()
            .Where(tm => tm.TeamId == project.TeamId)
            .Select(tm => tm.UserId)
            .ToListAsync(cancellationToken);

        var teamMemberSet = new HashSet<Guid>(teamMemberIds);

        // Get existing project member IDs
        var existingProjectMemberIds = await _context
            .ProjectMembers.AsNoTracking()
            .Where(pm => pm.ProjectId == projectId)
            .Select(pm => pm.UserId)
            .ToListAsync(cancellationToken);

        var existingProjectMemberSet = new HashSet<Guid>(existingProjectMemberIds);

        // Available = team members who are NOT project members
        var availableUserIds = teamMemberSet
            .Where(id => !existingProjectMemberSet.Contains(id))
            .ToHashSet();

        // Build query for users
        var query = _context.Users.AsNoTracking().Where(u => availableUserIds.Contains(u.Id));

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(u =>
                EF.Functions.Collate(u.Name, "SQL_Latin1_General_CP1_CI_AI").Contains(search)
                || EF.Functions.Collate(u.Email, "SQL_Latin1_General_CP1_CI_AI").Contains(search)
            );
        }

        // Get total count
        var total = await query.CountAsync(cancellationToken);

        // Apply pagination
        var items = await query
            .OrderBy(u => u.Name)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(u => new GetAvailableProjectMembersResult(
                u.Id,
                u.Name,
                u.Email,
                u.AvatarPicture != null ? u.AvatarPicture.Url : null
            ))
            .ToListAsync(cancellationToken);

        return new PaginationResult<GetAvailableProjectMembersResult>(
            pagination.Page,
            pagination.PageSize,
            total,
            items
        );
    }
}
