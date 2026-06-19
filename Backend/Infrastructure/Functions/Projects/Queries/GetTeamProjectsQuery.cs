using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Common.Models;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Projects.Queries;

public sealed record GetTeamProjectsQuery(
    Guid CurrentUserId,
    Guid TeamId,
    string? Search,
    PaginationRequest Pagination
) : IRequest<PaginationResult<TeamProjectItem>>;

public sealed record TeamProjectItem(
    Guid Id,
    Guid CreatorId,
    bool CanView,
    string Name,
    string? Description,
    string Visibility
);

public sealed class GetTeamProjectsQueryHandler
    : IRequestHandler<GetTeamProjectsQuery, PaginationResult<TeamProjectItem>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTeamProjectsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginationResult<TeamProjectItem>> Handle(
        GetTeamProjectsQuery request,
        CancellationToken cancellationToken
    )
    {
        // Check user is a team member
        var teamMember =
            await _unitOfWork
                .TeamMembers.ReadOnly()
                .Where(tm => tm.TeamId == request.TeamId && tm.UserId == request.CurrentUserId)
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new ForbiddenException("You are not a member of this team");

        // Get projects: public projects + private projects where user is a member;
        // Admins can see all projects in the team
        if (teamMember.Role == TeamMemberRole.Admin)
        {
            var projects = _unitOfWork
                .Projects.ReadOnly()
                .Where(p =>
                    p.TeamId == request.TeamId
                    && (string.IsNullOrEmpty(request.Search) || p.Name.Contains(request.Search))
                )
                .Select(p => new TeamProjectItem(
                    p.Id,
                    p.CreatorId,
                    true, // Admin can view all projects
                    p.Name,
                    p.Description,
                    p.Visibility.ToString()
                ));

            var projectCount = await projects.CountAsync(cancellationToken);

            var result = await projects
                .Take(request.Pagination.PageSize)
                .Skip((request.Pagination.Page - 1) * request.Pagination.PageSize)
                .ToListAsync(cancellationToken);

            return new PaginationResult<TeamProjectItem>(
                request.Pagination.Page,
                request.Pagination.PageSize,
                projectCount,
                result
            );
        }

        // Regular members: public projects + private projects they are members of

        var query = _unitOfWork
            .Projects.ReadOnly()
            .Where(p =>
                p.TeamId == request.TeamId
                && (
                    p.Visibility == ProjectVisibility.Public
                    || p.ProjectMembers.Any(pm => pm.UserId == request.CurrentUserId)
                )
                && (string.IsNullOrEmpty(request.Search) || p.Name.Contains(request.Search))
            );

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Select(p => new TeamProjectItem(
                p.Id,
                p.CreatorId,
                p.CreatorId == request.CurrentUserId
                    || p.ProjectMembers.Any(pm => pm.UserId == request.CurrentUserId), // Can view if creator or member
                p.Name,
                p.Description,
                p.Visibility.ToString()
            ))
            .Take(request.Pagination.PageSize)
            .Skip((request.Pagination.Page - 1) * request.Pagination.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginationResult<TeamProjectItem>(
            request.Pagination.Page,
            request.Pagination.PageSize,
            totalCount,
            items
        );
    }
}
