using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Behavior;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed record DeleteTeamCommand(
    Guid CurrentUserId,
    Guid TeamId,
    CancellationToken CancellationToken = default
) : IRequest, IRequireTeamRole
{
    TeamMemberRole[] IRequireTeamRole.AllowedRoles => [TeamMemberRole.Admin];
}

public sealed class DeleteTeamCommandHandler : IRequestHandler<DeleteTeamCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTeamCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteTeamCommand request, CancellationToken cancellationToken)
    {
        var team =
            await _unitOfWork
                .Teams.GetQuery()
                .FirstOrDefaultAsync(t => t.Id == request.TeamId, cancellationToken)
            ?? throw new NotFoundException(ErrorCodes.TeamNotFound);

        var members = await _unitOfWork
            .TeamMembers.GetQuery()
            .Where(tm => tm.TeamId == request.TeamId)
            .ToListAsync(cancellationToken);

        // Remove all project members from all projects in this team
        var projectIds = await _unitOfWork
            .Projects.GetQuery()
            .Where(p => p.TeamId == request.TeamId)
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);

        if (projectIds.Count > 0)
        {
            var projectMembers = await _unitOfWork
                .ProjectMembers.GetQuery()
                .Where(pm => projectIds.Contains(pm.ProjectId))
                .ToListAsync(cancellationToken);

            _unitOfWork.ProjectMembers.RemoveRange(projectMembers);
        }

        // Remove all projects in the team
        var projects = await _unitOfWork
            .Projects.GetQuery()
            .Where(p => p.TeamId == request.TeamId)
            .ToListAsync(cancellationToken);

        _unitOfWork.Projects.RemoveRange(projects);
        _unitOfWork.TeamMembers.RemoveRange(members);
        _unitOfWork.Teams.Remove(team);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
