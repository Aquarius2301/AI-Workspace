using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Projects.Commands;

public sealed record RemoveProjectMemberCommand(Guid CurrentUserId, Guid ProjectId, Guid UserId)
    : IRequest;

public sealed class RemoveProjectMemberCommandHandler : IRequestHandler<RemoveProjectMemberCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public RemoveProjectMemberCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        RemoveProjectMemberCommand request,
        CancellationToken cancellationToken
    )
    {
        var project =
            await _unitOfWork
                .Projects.GetQuery()
                .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken)
            ?? throw new NotFoundException(ErrorCodes.ProjectNotFound);

        // Check permissions: Admin (any project) OR Leader (if creatorId matches)
        var teamRole = await _unitOfWork
            .TeamMembers.GetQuery()
            .Where(tm => tm.TeamId == project.TeamId && tm.UserId == request.CurrentUserId)
            .Select(tm => tm.Role)
            .FirstOrDefaultAsync(cancellationToken);

        if (teamRole == TeamMemberRole.Admin)
        {
            // Admin can remove members from any project
        }
        else if (teamRole == TeamMemberRole.Leader && project.CreatorId == request.CurrentUserId)
        {
            // Leader can only remove members from projects they created
        }
        else
        {
            throw new ForbiddenException(
                "You do not have permission to remove members from this project"
            );
        }

        var projectMember =
            await _unitOfWork
                .ProjectMembers.GetQuery()
                .FirstOrDefaultAsync(
                    pm => pm.ProjectId == request.ProjectId && pm.UserId == request.UserId,
                    cancellationToken
                )
            ?? throw new NotFoundException(ErrorCodes.ProjectMemberNotFound);

        _unitOfWork.ProjectMembers.Remove(projectMember);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
