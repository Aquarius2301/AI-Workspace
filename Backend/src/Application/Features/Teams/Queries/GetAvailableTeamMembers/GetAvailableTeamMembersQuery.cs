using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Behaviors;
using AIWorkspace.Application.Common.Models;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Teams;

public sealed record GetAvailableTeamMembersQuery(
    Guid UserId,
    Guid TeamId,
    PaginationRequest Pagination,
    string? Search,
    CancellationToken CancellationToken
) : IRequest<PaginationResult<GetAvailableTeamMembersResult>>, IRequireTeamRole
{
    Guid IRequireTeamRole.TeamId => TeamId;
    Guid IRequireTeamRole.CurrentUserId => UserId;
    TeamMemberRole[] IRequireTeamRole.AllowedRoles =>
        [TeamMemberRole.Admin, TeamMemberRole.CoAdmin];
}

public sealed record GetAvailableTeamMembersResult(
    Guid Id,
    string Name,
    string Email,
    string? AvatarUrl
);

public sealed class GetAvailableTeamMembersQueryHandler
    : IRequestHandler<GetAvailableTeamMembersQuery, PaginationResult<GetAvailableTeamMembersResult>>
{
    private readonly IAppDbContext _context;

    public GetAvailableTeamMembersQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginationResult<GetAvailableTeamMembersResult>> Handle(
        GetAvailableTeamMembersQuery request,
        CancellationToken cancellationToken
    )
    {
        var teamId = request.TeamId;
        var search = request.Search;
        var pagination = request.Pagination;

        // Role authorization is handled by TeamRoleBehavior (Admin/CoAdmin only)

        // Build query at DB level — users not in this team
        var query = _context
            .Users.AsNoTracking()
            .Where(u => !_context.TeamMembers.Any(tm => tm.TeamId == teamId && tm.UserId == u.Id));

        // Apply search filter at DB level using SQL Server collation
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(u =>
                EF.Functions.Collate(u.Name, "SQL_Latin1_General_CP1_CI_AI").Contains(search)
                || EF.Functions.Collate(u.Email, "SQL_Latin1_General_CP1_CI_AI").Contains(search)
            );
        }

        // Get total count at DB level
        var total = await query.CountAsync(cancellationToken);

        // Apply pagination and projection at DB level — single round-trip
        var items = await query
            .OrderBy(u => u.Name)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(u => new GetAvailableTeamMembersResult(
                u.Id,
                u.Name,
                u.Email,
                u.AvatarPicture != null ? u.AvatarPicture.Url : null
            ))
            .ToListAsync(cancellationToken);

        return new PaginationResult<GetAvailableTeamMembersResult>(
            pagination.Page,
            pagination.PageSize,
            total,
            items
        );
    }
}
