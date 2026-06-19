using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Projects.Queries;

public sealed record GetTeamProjectsQuery(Guid CurrentUserId, Guid TeamId)
    : IRequest<List<TeamProjectItem>>;

public sealed record TeamProjectItem(
    Guid Id,
    Guid CreatorId,
    string Name,
    string? Description,
    string Visibility
);

public sealed class GetTeamProjectsQueryHandler
    : IRequestHandler<GetTeamProjectsQuery, List<TeamProjectItem>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTeamProjectsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<TeamProjectItem>> Handle(
        GetTeamProjectsQuery request,
        CancellationToken cancellationToken
    )
    {
        // Check user is a team member
        var isTeamMember = await _unitOfWork
            .TeamMembers.ReadOnly()
            .AnyAsync(
                tm => tm.TeamId == request.TeamId && tm.UserId == request.CurrentUserId,
                cancellationToken
            );

        if (!isTeamMember)
            throw new ForbiddenException("You are not a member of this team");

        // Get projects: public projects + private projects where user is a member
        var teamRole = await _unitOfWork
            .TeamMembers.ReadOnly()
            .Where(tm => tm.TeamId == request.TeamId && tm.UserId == request.CurrentUserId)
            .Select(tm => tm.Role)
            .FirstOrDefaultAsync(cancellationToken);

        var isAdminOrLeader = teamRole == TeamMemberRole.Admin || teamRole == TeamMemberRole.Leader;

        // Admins/Leaders can see all projects in the team
        if (isAdminOrLeader)
        {
            return await _unitOfWork
                .Projects.ReadOnly()
                .Where(p => p.TeamId == request.TeamId)
                .Select(p => new TeamProjectItem(
                    p.Id,
                    p.CreatorId,
                    p.Name,
                    p.Description,
                    p.Visibility.ToString()
                ))
                .ToListAsync(cancellationToken);
        }

        // Regular members: public projects + private projects they are members of
        var userProjectIds = await _unitOfWork
            .ProjectMembers.ReadOnly()
            .Where(pm => pm.Project.TeamId == request.TeamId && pm.UserId == request.CurrentUserId)
            .Select(pm => pm.ProjectId)
            .ToListAsync(cancellationToken);

        return await _unitOfWork
            .Projects.ReadOnly()
            .Where(p =>
                p.TeamId == request.TeamId
                && (p.Visibility == ProjectVisibility.Public || userProjectIds.Contains(p.Id))
            )
            .Select(p => new TeamProjectItem(
                p.Id,
                p.CreatorId,
                p.Name,
                p.Description,
                p.Visibility.ToString()
            ))
            .ToListAsync(cancellationToken);
    }
}
