using BusinessObject.Entities;
using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Documents.Commands;

public sealed record CreateDocumentCommand(
    Guid CurrentUserId,
    Guid ProjectId,
    string Title,
    string? Content
) : IRequest<DocumentCreatedResponse>;

public sealed record DocumentCreatedResponse(Guid Id);

public sealed class CreateDocumentCommandHandler
    : IRequestHandler<CreateDocumentCommand, DocumentCreatedResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateDocumentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DocumentCreatedResponse> Handle(
        CreateDocumentCommand request,
        CancellationToken cancellationToken
    )
    {
        var project =
            await _unitOfWork
                .Projects.GetQuery()
                .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken)
            ?? throw new NotFoundException("Project not found");

        // Check permissions: Admin (any project), Leader (if creatorId matches), or Member with write access
        var teamRole = await _unitOfWork
            .TeamMembers.GetQuery()
            .Where(tm => tm.TeamId == project.TeamId && tm.UserId == request.CurrentUserId)
            .Select(tm => tm.Role)
            .FirstOrDefaultAsync(cancellationToken);

        var isAdmin = teamRole == TeamMemberRole.Admin;
        var isLeader =
            teamRole == TeamMemberRole.Leader && project.CreatorId == request.CurrentUserId;
        var isProjectMember = await _unitOfWork
            .ProjectMembers.GetQuery()
            .AnyAsync(
                pm => pm.ProjectId == request.ProjectId && pm.UserId == request.CurrentUserId,
                cancellationToken
            );

        // Allow: Admin, Leader (own project), or any project member (write access)
        if (!isAdmin && !isLeader && !isProjectMember)
        {
            // For public projects, any team member can create documents
            if (project.Visibility != ProjectVisibility.Public)
            {
                throw new ForbiddenException(
                    "You do not have permission to create documents in this project"
                );
            }
        }

        var document = new Document
        {
            Id = Guid.NewGuid(),
            ProjectId = request.ProjectId,
            CreatorId = request.CurrentUserId,
            Title = request.Title,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow,
        };

        _unitOfWork.Documents.Add(document);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new DocumentCreatedResponse(document.Id);
    }
}
