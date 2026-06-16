using BusinessObject.Entities;
using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Documents.Commands;

public sealed record DeleteDocumentCommand(Guid CurrentUserId, Guid DocumentId) : IRequest<Unit>;

public sealed class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDocumentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(
        DeleteDocumentCommand request,
        CancellationToken cancellationToken
    )
    {
        var document =
            await _unitOfWork
                .Documents.GetQuery()
                .Include(d => d.Project)
                .FirstOrDefaultAsync(d => d.Id == request.DocumentId, cancellationToken)
            ?? throw new NotFoundException("Document not found");

        // Check permissions: Admin (any project), Leader (if creatorId matches), or Creator
        var teamRole = await _unitOfWork
            .TeamMembers.GetQuery()
            .Where(tm => tm.TeamId == document.Project.TeamId && tm.UserId == request.CurrentUserId)
            .Select(tm => tm.Role)
            .FirstOrDefaultAsync(cancellationToken);

        var isAdmin = teamRole == TeamMemberRole.Admin;
        var isLeader =
            teamRole == TeamMemberRole.Leader
            && document.Project.CreatorId == request.CurrentUserId;
        var isCreator = document.CreatorId == request.CurrentUserId;

        if (!isAdmin && !isLeader && !isCreator)
        {
            throw new ForbiddenException("You do not have permission to delete this document");
        }

        _unitOfWork.Documents.Remove(document);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
