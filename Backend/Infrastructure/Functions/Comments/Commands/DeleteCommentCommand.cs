using BusinessObject.Entities;
using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Comments.Commands;

public sealed record DeleteCommentCommand(Guid CurrentUserId, Guid CommentId) : IRequest<Unit>;

public sealed class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCommentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(
        DeleteCommentCommand request,
        CancellationToken cancellationToken
    )
    {
        var comment =
            await _unitOfWork
                .Comments.GetQuery()
                .FirstOrDefaultAsync(c => c.Id == request.CommentId, cancellationToken)
            ?? throw new NotFoundException("Comment not found");

        // Permission: Admin, Leader (own project), or Comment creator
        if (comment.CreatorId == request.CurrentUserId)
        {
            // Creator can always delete their own comment
        }
        else
        {
            // Check if user is Admin/Leader of the project containing this comment
            // We need to determine the reference type to find the project
            Guid projectId;

            if (comment.ReferenceType == ReferenceType.TaskItem)
            {
                var task =
                    await _unitOfWork
                        .TaskItems.GetQuery()
                        .FirstOrDefaultAsync(t => t.Id == comment.ReferenceId, cancellationToken)
                    ?? throw new NotFoundException("Referenced task not found");
                projectId = task.ProjectId;
            }
            else // Document
            {
                var document =
                    await _unitOfWork
                        .Documents.GetQuery()
                        .FirstOrDefaultAsync(d => d.Id == comment.ReferenceId, cancellationToken)
                    ?? throw new NotFoundException("Referenced document not found");
                projectId = document.ProjectId;
            }

            var project =
                await _unitOfWork
                    .Projects.GetQuery()
                    .FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken)
                ?? throw new NotFoundException("Project not found");

            var teamRole = await _unitOfWork
                .TeamMembers.GetQuery()
                .Where(tm => tm.TeamId == project.TeamId && tm.UserId == request.CurrentUserId)
                .Select(tm => tm.Role)
                .FirstOrDefaultAsync(cancellationToken);

            var isAdmin = teamRole == TeamMemberRole.Admin;
            var isLeader =
                teamRole == TeamMemberRole.Leader && project.CreatorId == request.CurrentUserId;

            if (!isAdmin && !isLeader)
            {
                throw new ForbiddenException("You do not have permission to delete this comment");
            }
        }

        _unitOfWork.Comments.Remove(comment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
