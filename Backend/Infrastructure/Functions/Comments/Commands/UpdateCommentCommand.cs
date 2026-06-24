using BusinessObject.Entities;
using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Comments.Commands;

public sealed record UpdateCommentCommand(Guid CurrentUserId, Guid CommentId, string Content)
    : IRequest<Unit>;

public sealed class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCommentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(
        UpdateCommentCommand request,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            throw new BadRequestException(ErrorCodes.CommentContentRequired);

        var comment =
            await _unitOfWork
                .Comments.GetQuery()
                .FirstOrDefaultAsync(c => c.Id == request.CommentId, cancellationToken)
            ?? throw new NotFoundException(ErrorCodes.CommentNotFound);

        // Only the creator can update their own comment
        if (comment.CreatorId != request.CurrentUserId)
        {
            throw new ForbiddenException(ErrorCodes.OwnCommentOnly);
        }

        comment.Content = request.Content;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
