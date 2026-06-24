using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Projects.Queries;

public sealed record GetProjectDetailQuery(Guid CurrentUserId, Guid ProjectId)
    : IRequest<ProjectDetailResponse>;

public sealed record ProjectDetailResponse(
    Guid Id,
    Guid TeamId,
    Guid CreatorId,
    string Name,
    string? Description,
    string Visibility
);

public sealed class GetProjectDetailQueryHandler
    : IRequestHandler<GetProjectDetailQuery, ProjectDetailResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProjectDetailQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ProjectDetailResponse> Handle(
        GetProjectDetailQuery request,
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

        // If private, check user is a project member (unless Admin/Leader)
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

        return new ProjectDetailResponse(
            project.Id,
            project.TeamId,
            project.CreatorId,
            project.Name,
            project.Description,
            project.Visibility.ToString()
        );
    }
}
