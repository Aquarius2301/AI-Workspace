using BusinessObject.Entities;
using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Comments.Commands;

public sealed record CreateDocumentCommentCommand(
    Guid CurrentUserId,
    Guid DocumentId,
    string Content
) : IRequest;

public sealed class CreateDocumentCommentCommandHandler
    : IRequestHandler<CreateDocumentCommentCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateDocumentCommentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        CreateDocumentCommentCommand request,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            throw new BadRequestException("Comment content is required");

        var document =
            await _unitOfWork
                .Documents.GetQuery()
                .Include(d => d.Project)
                .FirstOrDefaultAsync(d => d.Id == request.DocumentId, cancellationToken)
            ?? throw new NotFoundException("Document not found");

        // Check user is a member of the project's team
        var isTeamMember = await _unitOfWork
            .TeamMembers.GetQuery()
            .AnyAsync(
                tm => tm.TeamId == document.Project.TeamId && tm.UserId == request.CurrentUserId,
                cancellationToken
            );

        if (!isTeamMember)
            throw new ForbiddenException("You are not a member of this team");

        // If private project, check project membership unless Admin/Leader
        if (document.Project.Visibility == ProjectVisibility.Private)
        {
            var teamRole = await _unitOfWork
                .TeamMembers.GetQuery()
                .Where(tm =>
                    tm.TeamId == document.Project.TeamId && tm.UserId == request.CurrentUserId
                )
                .Select(tm => tm.Role)
                .FirstOrDefaultAsync(cancellationToken);

            var isAdminOrLeader =
                teamRole == TeamMemberRole.Admin || teamRole == TeamMemberRole.Leader;

            if (!isAdminOrLeader)
            {
                var isProjectMember = await _unitOfWork
                    .ProjectMembers.GetQuery()
                    .AnyAsync(
                        pm =>
                            pm.ProjectId == document.ProjectId
                            && pm.UserId == request.CurrentUserId,
                        cancellationToken
                    );

                if (!isProjectMember)
                    throw new ForbiddenException("You are not a member of this private project");
            }
        }

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            CreatorId = request.CurrentUserId,
            ReferenceType = ReferenceType.Document,
            ReferenceId = request.DocumentId,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow,
        };

        _unitOfWork.Comments.Add(comment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
