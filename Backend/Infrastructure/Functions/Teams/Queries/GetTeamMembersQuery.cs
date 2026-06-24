using BusinessObject.Entities;
using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Teams;

public sealed record GetTeamMembersQuery(
    Guid TeamId,
    PaginationRequest Pagination,
    string? Search,
    TeamMemberRole? Role
) : IRequest<PaginationResult<TeamMemberItem>>;

public sealed record TeamMemberItem(
    Guid UserId,
    string UserName,
    string Role,
    DateTime JoinedAt,
    string Email,
    DateTime? LastActiveAt
);

public sealed class GetTeamMembersQueryHandler
    : IRequestHandler<GetTeamMembersQuery, PaginationResult<TeamMemberItem>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTeamMembersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginationResult<TeamMemberItem>> Handle(
        GetTeamMembersQuery request,
        CancellationToken cancellationToken
    )
    {
        var query = _unitOfWork.TeamMembers.ReadOnly().Where(tm => tm.TeamId == request.TeamId);

        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(m =>
                m.User.Name.Contains(request.Search) || m.User.Email.Contains(request.Search)
            );
        }

        if (request.Role.HasValue)
        {
            query = query.Where(m => m.Role == request.Role.Value);
        }

        var count = await query.CountAsync(cancellationToken);

        var members = await query
            .OrderBy(x =>
                x.Role == TeamMemberRole.Admin ? 1
                : x.Role == TeamMemberRole.Leader ? 2
                : 3
            ) // Order by Role (Admin first, then Leader, then Member)
            .ThenBy(x => x.User.Name) // Then by UserName
            .Select(tm => new TeamMemberItem(
                tm.UserId,
                tm.User.Name,
                tm.Role.ToString(),
                tm.JoinedAt,
                tm.User.Email,
                tm.User.LastActiveAt
            ))
            .Skip((request.Pagination.Page - 1) * request.Pagination.PageSize)
            .Take(request.Pagination.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginationResult<TeamMemberItem>(
            request.Pagination.Page,
            request.Pagination.PageSize,
            count,
            members
        );
    }
}
