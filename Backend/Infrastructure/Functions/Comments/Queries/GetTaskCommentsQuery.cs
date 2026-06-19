using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Comments.Queries;

public sealed record GetTaskCommentsQuery(Guid CurrentUserId, Guid TaskId)
    : IRequest<List<TaskCommentResponse>>;

public sealed record TaskCommentResponse(
    Guid Id,
    Guid CreatorId,
    string CreatorName,
    string? AvatarUrl,
    string Content,
    DateTime CreatedAt
);

public sealed class GetTaskCommentsQueryHandler
    : IRequestHandler<GetTaskCommentsQuery, List<TaskCommentResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTaskCommentsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<TaskCommentResponse>> Handle(
        GetTaskCommentsQuery request,
        CancellationToken cancellationToken
    )
    {
        var task =
            await _unitOfWork
                .TaskItems.ReadOnly()
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken)
            ?? throw new NotFoundException("Task not found");

        // Check user is a member of the project's team
        var isTeamMember = await _unitOfWork
            .TeamMembers.ReadOnly()
            .AnyAsync(
                tm => tm.TeamId == task.Project.TeamId && tm.UserId == request.CurrentUserId,
                cancellationToken
            );

        if (!isTeamMember)
            throw new ForbiddenException("You are not a member of this team");

        // If private project, check project membership unless Admin/Leader
        if (task.Project.Visibility == ProjectVisibility.Private)
        {
            var teamRole = await _unitOfWork
                .TeamMembers.ReadOnly()
                .Where(tm => tm.TeamId == task.Project.TeamId && tm.UserId == request.CurrentUserId)
                .Select(tm => tm.Role)
                .FirstOrDefaultAsync(cancellationToken);

            var isAdminOrLeader =
                teamRole == TeamMemberRole.Admin || teamRole == TeamMemberRole.Leader;

            if (!isAdminOrLeader)
            {
                var isProjectMember = await _unitOfWork
                    .ProjectMembers.ReadOnly()
                    .AnyAsync(
                        pm => pm.ProjectId == task.ProjectId && pm.UserId == request.CurrentUserId,
                        cancellationToken
                    );

                if (!isProjectMember)
                    throw new ForbiddenException("You are not a member of this private project");
            }
        }

        var comments = await _unitOfWork
            .Comments.ReadOnly()
            .Include(c => c.Creator)
            .Where(c =>
                c.ReferenceType == ReferenceType.TaskItem && c.ReferenceId == request.TaskId
            )
            .OrderBy(c => c.CreatedAt)
            .Select(c => new TaskCommentResponse(
                c.Id,
                c.CreatorId,
                c.Creator.Name,
                c.Creator.AvatarUrl,
                c.Content,
                c.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        return comments;
    }
}
