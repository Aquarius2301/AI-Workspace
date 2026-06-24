using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Comments.Queries;

public sealed record GetDocumentCommentsQuery(Guid CurrentUserId, Guid DocumentId)
    : IRequest<List<DocumentCommentResponse>>;

public sealed record DocumentCommentResponse(
    Guid Id,
    Guid CreatorId,
    string CreatorName,
    string? AvatarUrl,
    string Content,
    DateTime CreatedAt
);

public sealed class GetDocumentCommentsQueryHandler
    : IRequestHandler<GetDocumentCommentsQuery, List<DocumentCommentResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetDocumentCommentsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<DocumentCommentResponse>> Handle(
        GetDocumentCommentsQuery request,
        CancellationToken cancellationToken
    )
    {
        var document =
            await _unitOfWork
                .Documents.ReadOnly()
                .Include(d => d.Project)
                .FirstOrDefaultAsync(d => d.Id == request.DocumentId, cancellationToken)
            ?? throw new NotFoundException(ErrorCodes.DocumentNotFound);

        // Check user is a member of the project's team
        var isTeamMember = await _unitOfWork
            .TeamMembers.ReadOnly()
            .AnyAsync(
                tm => tm.TeamId == document.Project.TeamId && tm.UserId == request.CurrentUserId,
                cancellationToken
            );

        if (!isTeamMember)
            throw new ForbiddenException(ErrorCodes.NotTeamMember);

        // If private project, check project membership unless Admin/Leader
        if (document.Project.Visibility == ProjectVisibility.Private)
        {
            var teamRole = await _unitOfWork
                .TeamMembers.ReadOnly()
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
                    .ProjectMembers.ReadOnly()
                    .AnyAsync(
                        pm =>
                            pm.ProjectId == document.ProjectId
                            && pm.UserId == request.CurrentUserId,
                        cancellationToken
                    );

                if (!isProjectMember)
                    throw new ForbiddenException(ErrorCodes.NotPrivateProjectMember);
            }
        }

        var comments = await _unitOfWork
            .Comments.ReadOnly()
            .Include(c => c.Creator)
            .Where(c =>
                c.ReferenceType == ReferenceType.Document && c.ReferenceId == request.DocumentId
            )
            .OrderBy(c => c.CreatedAt)
            .Select(c => new DocumentCommentResponse(
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
