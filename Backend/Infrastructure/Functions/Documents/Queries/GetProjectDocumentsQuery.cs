using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Common.Models;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Documents.Queries;

public sealed record GetProjectDocumentsQuery(
    Guid CurrentUserId,
    Guid ProjectId,
    PaginationRequest Pagination,
    string? SearchTitle
) : IRequest<PaginationResult<ProjectDocumentResponse>>;

public sealed record ProjectDocumentResponse(
    Guid Id,
    string Title,
    string? CreatorName,
    DateTime CreatedAt
);

public sealed class GetProjectDocumentsQueryHandler
    : IRequestHandler<GetProjectDocumentsQuery, PaginationResult<ProjectDocumentResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProjectDocumentsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginationResult<ProjectDocumentResponse>> Handle(
        GetProjectDocumentsQuery request,
        CancellationToken cancellationToken
    )
    {
        var project =
            await _unitOfWork
                .Projects.ReadOnly()
                .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken)
            ?? throw new NotFoundException(ErrorCodes.ProjectNotFound);

        // Check access: team member (for public) or Admin/Leader, or project member (for private)
        var isTeamMember = await _unitOfWork
            .TeamMembers.ReadOnly()
            .AnyAsync(
                tm => tm.TeamId == project.TeamId && tm.UserId == request.CurrentUserId,
                cancellationToken
            );

        if (!isTeamMember)
            throw new ForbiddenException(ErrorCodes.NotTeamMember);

        if (project.Visibility == ProjectVisibility.Private)
        {
            var teamRole = await _unitOfWork
                .TeamMembers.ReadOnly()
                .Where(tm => tm.TeamId == project.TeamId && tm.UserId == request.CurrentUserId)
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
                            pm.ProjectId == request.ProjectId && pm.UserId == request.CurrentUserId,
                        cancellationToken
                    );

                if (!isProjectMember)
                    throw new ForbiddenException(ErrorCodes.NotPrivateProjectMember);
            }
        }

        // Build query
        var query = _unitOfWork
            .Documents.ReadOnly()
            .Include(d => d.Creator)
            .Where(d => d.ProjectId == request.ProjectId);

        if (!string.IsNullOrWhiteSpace(request.SearchTitle))
        {
            var search = request.SearchTitle.Trim().ToLower();
            query = query.Where(d => d.Title.ToLower().Contains(search));
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(d => d.CreatedAt)
            .Skip((request.Pagination.Page - 1) * request.Pagination.PageSize)
            .Take(request.Pagination.PageSize)
            .Select(d => new ProjectDocumentResponse(d.Id, d.Title, d.Creator.Name, d.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PaginationResult<ProjectDocumentResponse>(
            request.Pagination.Page,
            request.Pagination.PageSize,
            total,
            items
        );
    }
}
