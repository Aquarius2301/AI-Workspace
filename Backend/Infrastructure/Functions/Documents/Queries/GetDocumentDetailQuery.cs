using BusinessObject.Entities;
using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Documents.Queries;

public sealed record GetDocumentDetailQuery(Guid CurrentUserId, Guid DocumentId)
    : IRequest<DocumentDetailResponse>;

public sealed record DocumentDetailResponse(
    Guid Id,
    Guid ProjectId,
    string Title,
    string? Content,
    Guid CreatorId,
    string? CreatorName,
    DateTime CreatedAt
);

public sealed class GetDocumentDetailQueryHandler
    : IRequestHandler<GetDocumentDetailQuery, DocumentDetailResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetDocumentDetailQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DocumentDetailResponse> Handle(
        GetDocumentDetailQuery request,
        CancellationToken cancellationToken
    )
    {
        var document =
            await _unitOfWork
                .Documents.GetQuery()
                .Include(d => d.Project)
                .Include(d => d.Creator)
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

        // If private project, check user is project member unless Admin/Leader
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

        return new DocumentDetailResponse(
            document.Id,
            document.ProjectId,
            document.Title,
            document.Content,
            document.CreatorId,
            document.Creator.Name,
            document.CreatedAt
        );
    }
}
