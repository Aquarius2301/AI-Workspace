using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Application.Common.Models;
using AIWorkspace.Application.Helpers;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Teams;

public sealed record GetTeamMembersQuery(
    Guid TeamId,
    Guid CurrentUserId,
    PaginationRequest Pagination,
    string? Search,
    TeamMemberRole? Role,
    CancellationToken CancellationToken
) : IRequest<PaginationResult<GetTeamMembersResult>>;

public sealed record GetTeamMembersResult(
    Guid UserId,
    string UserName,
    string Email,
    string? AvatarUrl,
    TeamMemberRole Role,
    DateTimeOffset JoinedAt,
    DateTimeOffset? LastActiveAt
);

public sealed class GetTeamMembersQueryHandler
    : IRequestHandler<GetTeamMembersQuery, PaginationResult<GetTeamMembersResult>>
{
    private readonly IAppDbContext _context;

    public GetTeamMembersQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginationResult<GetTeamMembersResult>> Handle(
        GetTeamMembersQuery request,
        CancellationToken cancellationToken
    )
    {
        var teamId = request.TeamId;
        var currentUserId = request.CurrentUserId;
        var search = request.Search;
        var role = request.Role;
        var pagination = request.Pagination;

        // Verify the current user is a member of this team (authorization check)
        var isMember = await _context
            .TeamMembers.AsNoTracking()
            .AnyAsync(tm => tm.TeamId == teamId && tm.UserId == currentUserId, cancellationToken);

        if (!isMember)
        {
            throw new NotFoundException(ErrorCodes.TeamNotFound);
        }

        // Build query with filters at DB level to avoid loading all data into memory
        IQueryable<Domain.Entities.TeamMember> query = _context
            .TeamMembers.AsNoTracking()
            .Where(tm => tm.TeamId == teamId)
            .Include(tm => tm.User)
                .ThenInclude(u => u.AvatarPicture);

        // Apply role filter at DB level if provided
        if (role.HasValue)
        {
            query = query.Where(tm => tm.Role == role.Value);
        }

        // Apply search filter using SQL Server collation for diacritics-insensitive search
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(tm =>
                EF.Functions.Collate(tm.User.Name, "SQL_Latin1_General_CP1_CI_AI").Contains(search)
                || EF.Functions.Collate(tm.User.Email, "SQL_Latin1_General_CP1_CI_AI")
                    .Contains(search)
            );
        }

        // Get total count after filters applied
        var total = await query.CountAsync(cancellationToken);

        // Apply pagination and ordering at DB level
        var items = await query
            .OrderBy(tm =>
                tm.Role == TeamMemberRole.Admin ? 0
                : tm.Role == TeamMemberRole.CoAdmin ? 1
                : 2
            )
            .ThenBy(tm => tm.User.Name)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(tm => new GetTeamMembersResult(
                tm.UserId,
                tm.User.Name,
                tm.User.Email,
                tm.User.AvatarPicture != null ? tm.User.AvatarPicture.Url : null,
                tm.Role,
                tm.JoinedAt,
                tm.User.LastActiveAt
            ))
            .ToListAsync(cancellationToken);

        return new PaginationResult<GetTeamMembersResult>(
            pagination.Page,
            pagination.PageSize,
            total,
            items
        );
    }
}
