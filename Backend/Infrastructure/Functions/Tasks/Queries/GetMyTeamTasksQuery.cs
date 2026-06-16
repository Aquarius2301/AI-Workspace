using BusinessObject.Entities;
using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Tasks.Queries;

public sealed record GetMyTeamTasksQuery(Guid CurrentUserId, Guid TeamId)
    : IRequest<List<MyTaskItemResponse>>;

public sealed record MyTaskItemResponse(
    Guid Id,
    Guid ProjectId,
    string ProjectName,
    string Title,
    string? Description,
    int Priority,
    string Status,
    DateTime CreatedAt,
    DateTime? DueDate
);

public sealed class GetMyTeamTasksQueryHandler
    : IRequestHandler<GetMyTeamTasksQuery, List<MyTaskItemResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMyTeamTasksQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<MyTaskItemResponse>> Handle(
        GetMyTeamTasksQuery request,
        CancellationToken cancellationToken
    )
    {
        // Check user is a member of the team
        var isTeamMember = await _unitOfWork
            .TeamMembers.GetQuery()
            .AnyAsync(
                tm => tm.TeamId == request.TeamId && tm.UserId == request.CurrentUserId,
                cancellationToken
            );

        if (!isTeamMember)
            throw new ForbiddenException("You are not a member of this team");

        // Get all tasks assigned to the current user within projects belonging to this team
        var teamProjectIds = _unitOfWork
            .Projects.GetQuery()
            .Where(p => p.TeamId == request.TeamId)
            .Select(p => p.Id);

        var tasks = await _unitOfWork
            .TaskItems.GetQuery()
            .Include(t => t.Project)
            .Where(t =>
                t.AssignedToId == request.CurrentUserId && teamProjectIds.Contains(t.ProjectId)
            )
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new MyTaskItemResponse(
                t.Id,
                t.ProjectId,
                t.Project.Name,
                t.Title,
                t.Description,
                t.Priority,
                t.Status.ToString(),
                t.CreatedAt,
                t.DueDate
            ))
            .ToListAsync(cancellationToken);

        return tasks;
    }
}
