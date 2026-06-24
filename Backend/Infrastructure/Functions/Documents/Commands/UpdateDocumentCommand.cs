using BusinessObject.Entities;
using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Documents.Commands;

public sealed record UpdateDocumentCommand(
    Guid CurrentUserId,
    Guid DocumentId,
    string? Title,
    string? Content
) : IRequest<Unit>;

public sealed class UpdateDocumentCommandHandler : IRequestHandler<UpdateDocumentCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDocumentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(
        UpdateDocumentCommand request,
        CancellationToken cancellationToken
    )
    {
        var document =
            await _unitOfWork
                .Documents.GetQuery()
                .Include(d => d.Project)
                .FirstOrDefaultAsync(d => d.Id == request.DocumentId, cancellationToken)
            ?? throw new NotFoundException(ErrorCodes.DocumentNotFound);

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
            throw new ForbiddenException(ErrorCodes.NoPermissionUpdateDocument);
        }

        if (!string.IsNullOrWhiteSpace(request.Title))
            document.Title = request.Title;

        if (request.Content is not null)
            document.Content = request.Content;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
