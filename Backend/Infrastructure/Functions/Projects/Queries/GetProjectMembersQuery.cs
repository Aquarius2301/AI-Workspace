using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Projects.Queries;

public sealed record GetProjectMembersQuery(Guid CurrentUserId, Guid ProjectId)
    : IRequest<List<ProjectMemberItem>>;

public sealed record ProjectMemberItem(
    Guid UserId,
    string UserName,
    string? Email,
    DateTime JoinedAt
);

public sealed class GetProjectMembersQueryHandler
    : IRequestHandler<GetProjectMembersQuery, List<ProjectMemberItem>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProjectMembersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<ProjectMemberItem>> Handle(
        GetProjectMembersQuery request,
        CancellationToken cancellationToken
    )
    {
        var project =
            await _unitOfWork
                .Projects.ReadOnly()
                .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken)
            ?? throw new NotFoundException(ErrorCodes.ProjectNotFound);

        // Check user is a team member
        var isTeamMember = await _unitOfWork
            .TeamMembers.ReadOnly()
            .AnyAsync(
                tm => tm.TeamId == project.TeamId && tm.UserId == request.CurrentUserId,
                cancellationToken
            );

        if (!isTeamMember)
            throw new ForbiddenException(ErrorCodes.NotTeamMember);

        // If private project, check user is a member (unless Admin/Leader)
        if (project.Visibility == ProjectVisibility.Private)
        {
            var teamRole = await _unitOfWork
                .TeamMembers.ReadOnly()
                .Where(tm => tm.TeamId == project.TeamId && tm.UserId == request.CurrentUserId)
                .Select(tm => tm.Role)
                .FirstOrDefaultAsync(cancellationToken);

            var isAdminOrLeader =
                teamRole == TeamMemberRole.Admin || teamRole == TeamMemberRole.Leader;

            if (!isAdminOrLeader)
            {
                var isProjectMember = await _unitOfWork
                    .ProjectMembers.ReadOnly()
                    .AnyAsync(
                        pm =>
                            pm.ProjectId == request.ProjectId && pm.UserId == request.CurrentUserId,
                        cancellationToken
                    );

                if (!isProjectMember)
                    throw new ForbiddenException(ErrorCodes.NotPrivateProjectMember);
            }
        }

        return await _unitOfWork
            .ProjectMembers.ReadOnly()
            .Where(pm => pm.ProjectId == request.ProjectId)
            .Select(pm => new ProjectMemberItem(
                pm.UserId,
                pm.User.Name,
                pm.User.Email,
                pm.JoinedAt
            ))
            .ToListAsync(cancellationToken);
    }
}
