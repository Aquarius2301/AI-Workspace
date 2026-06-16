using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Projects.Queries;

public sealed record GetAvailableProjectMembersQuery(
    Guid CurrentUserId,
    Guid TeamId,
    Guid ProjectId
) : IRequest<List<AvailableMemberItem>>;

public sealed record AvailableMemberItem(Guid UserId, string UserName, string? Email);

public sealed class GetAvailableProjectMembersQueryHandler
    : IRequestHandler<GetAvailableProjectMembersQuery, List<AvailableMemberItem>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAvailableProjectMembersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<AvailableMemberItem>> Handle(
        GetAvailableProjectMembersQuery request,
        CancellationToken cancellationToken
    )
    {
        var project =
            await _unitOfWork
                .Projects.GetQuery()
                .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken)
            ?? throw new NotFoundException("Project not found");

        // Check permissions: Admin (any project) OR Leader (if creatorId matches)
        var teamRole = await _unitOfWork
            .TeamMembers.GetQuery()
            .Where(tm => tm.TeamId == request.TeamId && tm.UserId == request.CurrentUserId)
            .Select(tm => tm.Role)
            .FirstOrDefaultAsync(cancellationToken);

        if (teamRole == TeamMemberRole.Admin)
        {
            // Admin can manage any project's members
        }
        else if (teamRole == TeamMemberRole.Leader && project.CreatorId == request.CurrentUserId)
        {
            // Leader can only manage projects they created
        }
        else
        {
            throw new ForbiddenException(
                "You do not have permission to view available members for this project"
            );
        }

        // Get team members who are NOT already in the project
        var existingProjectMemberIds = await _unitOfWork
            .ProjectMembers.GetQuery()
            .Where(pm => pm.ProjectId == request.ProjectId)
            .Select(pm => pm.UserId)
            .ToListAsync(cancellationToken);

        return await _unitOfWork
            .TeamMembers.GetQuery()
            .Where(tm =>
                tm.TeamId == request.TeamId && !existingProjectMemberIds.Contains(tm.UserId)
            )
            .Select(tm => new AvailableMemberItem(tm.UserId, tm.User.Name, tm.User.Email))
            .ToListAsync(cancellationToken);
    }
}
