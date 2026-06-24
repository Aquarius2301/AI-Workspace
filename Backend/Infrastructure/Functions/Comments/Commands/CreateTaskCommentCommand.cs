using BusinessObject.Entities;
using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Comments.Commands;

public sealed record CreateTaskCommentCommand(Guid CurrentUserId, Guid TaskId, string Content)
    : IRequest;

public sealed class CreateTaskCommentCommandHandler : IRequestHandler<CreateTaskCommentCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateTaskCommentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(CreateTaskCommentCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            throw new BadRequestException(ErrorCodes.CommentContentRequired);

        var task =
            await _unitOfWork
                .TaskItems.GetQuery()
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken)
            ?? throw new NotFoundException(ErrorCodes.TaskNotFound);

        // Check user is a member of the project's team
        var isTeamMember = await _unitOfWork
            .TeamMembers.GetQuery()
            .AnyAsync(
                tm => tm.TeamId == task.Project.TeamId && tm.UserId == request.CurrentUserId,
                cancellationToken
            );

        if (!isTeamMember)
            throw new ForbiddenException(ErrorCodes.NotTeamMember);

        // If private project, check project membership unless Admin/Leader
        if (task.Project.Visibility == ProjectVisibility.Private)
        {
            var teamRole = await _unitOfWork
                .TeamMembers.GetQuery()
                .Where(tm => tm.TeamId == task.Project.TeamId && tm.UserId == request.CurrentUserId)
                .Select(tm => tm.Role)
                .FirstOrDefaultAsync(cancellationToken);

            var isAdminOrLeader =
                teamRole == TeamMemberRole.Admin || teamRole == TeamMemberRole.Leader;

            if (!isAdminOrLeader)
            {
                var isProjectMember = await _unitOfWork
                    .ProjectMembers.GetQuery()
                    .AnyAsync(
                        pm => pm.ProjectId == task.ProjectId && pm.UserId == request.CurrentUserId,
                        cancellationToken
                    );

                if (!isProjectMember)
                    throw new ForbiddenException(ErrorCodes.NotPrivateProjectMember);
            }
        }

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            CreatorId = request.CurrentUserId,
            ReferenceType = ReferenceType.TaskItem,
            ReferenceId = request.TaskId,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow,
        };

        _unitOfWork.Comments.Add(comment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
