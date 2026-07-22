using AIWorkspace.Api.Helpers;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Application.Features.Tasks;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIWorkspace.Api.Controllers;

/// <summary>
/// Controller for task-level operations within a project.
/// All endpoints require authentication and are scoped under api/projects/{{projectId}}/tasks.
/// </summary>
[ApiController]
[Route("api/projects/{projectId:guid}/tasks")]
[Authorize]
public class TaskController : ControllerBase
{
    private readonly IMediator _mediator;

    public TaskController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new task within a project.
    /// </summary>
    /// <remarks>
    /// This action requires authentication. The user must have the <b>Admin</b> or <b>CoAdmin</b>
    /// role in the project's team, or the <b>Leader</b> role in the project.
    /// </remarks>
    /// <param name="projectId">The ID of the project to create the task in.</param>
    /// <param name="request">The request body containing task details.</param>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <response code="200">Returns the created task item.</response>
    /// <response code="400">Title is required (BadRequest).</response>
    /// <response code="401">User is not authenticated (Unauthorized).</response>
    /// <response code="403">User does not have sufficient role (Forbidden).</response>
    /// <response code="404">Project not found (ProjectNotFound).</response>
    [HttpPost]
    public async Task<IActionResult> CreateTask(
        Guid projectId,
        [FromBody] CreateTaskRequest request,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new BadRequestException(ErrorCodes.TaskTitleRequired);

        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(
            new CreateTaskCommand(
                userId,
                projectId,
                request.Title,
                request.Description,
                request.AssignedToId,
                request.Priority,
                request.DueDate,
                cancellationToken
            ),
            cancellationToken
        );

        return Ok(result);
    }

    /// <summary>
    /// Updates the status of a task by the assigned user.
    /// </summary>
    /// <remarks>
    /// This action requires authentication. Only the user assigned to the task can update its status.
    /// The user must have access to the project (team Admin/CoAdmin or project member).
    /// This endpoint is for individual users updating their own task status.
    /// For admin/leader-driven status overrides, a separate endpoint will be provided.
    /// </remarks>
    /// <param name="projectId">The ID of the project containing the task.</param>
    /// <param name="taskId">The ID of the task to update.</param>
    /// <param name="request">The request body containing the new status.</param>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <response code="200">Returns the updated task item.</response>
    /// <response code="400">Invalid status value provided (BadRequest).</response>
    /// <response code="401">User is not authenticated (Unauthorized).</response>
    /// <response code="403">User is not the assigned user of the task (Forbidden).</response>
    /// <response code="404">Task not found (TaskNotFound).</response>
    [HttpPatch("{taskId:guid}/status")]
    public async Task<IActionResult> UpdateMyTaskStatus(
        Guid projectId,
        Guid taskId,
        [FromBody] UpdateMyTaskStatusRequest request,
        CancellationToken cancellationToken
    )
    {
        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(
            new UpdateMyTaskStatusCommand(
                userId,
                projectId,
                taskId,
                request.Status,
                cancellationToken
            ),
            cancellationToken
        );

        return Ok(result);
    }

    [HttpPut("{taskId:guid}")]
    public async Task<IActionResult> UpdateTask(
        Guid projectId,
        Guid taskId,
        [FromBody] UpdateTaskRequest request,
        CancellationToken cancellationToken
    )
    {
        if (request.Title is not null && string.IsNullOrWhiteSpace(request.Title))
            throw new BadRequestException(ErrorCodes.TaskTitleRequired);

        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(
            new UpdateTaskCommand(
                userId,
                projectId,
                taskId,
                request.Title,
                request.Description,
                request.AssignedToId,
                request.Priority,
                request.DueDate,
                cancellationToken
            ),
            cancellationToken
        );

        return Ok(result);
    }

    [HttpDelete("{taskId:guid}")]
    public async Task<IActionResult> DeleteTask(
        Guid projectId,
        Guid taskId,
        CancellationToken cancellationToken
    )
    {
        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(
            new DeleteTaskCommand(userId, projectId, taskId, cancellationToken),
            cancellationToken
        );

        return Ok();
    }

    /// <summary>
    /// Updates the status of any task in the project by an admin/leader.
    /// </summary>
    /// <remarks>
    /// This action requires authentication. The user must have <b>Admin</b> or <b>CoAdmin</b>
    /// role in the project's team, or the <b>Leader</b> role in the project.
    /// Unlike <c>UpdateMyTaskStatus</c>, this endpoint does not restrict by assigned user.
    /// </remarks>
    [HttpPatch("{taskId:guid}/admin-status")]
    public async Task<IActionResult> UpdateTaskStatusByAdmin(
        Guid projectId,
        Guid taskId,
        [FromBody] UpdateTaskStatusByAdminRequest request,
        CancellationToken cancellationToken
    )
    {
        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(
            new UpdateTaskStatusByAdminCommand(
                userId,
                projectId,
                taskId,
                request.Status,
                cancellationToken
            ),
            cancellationToken
        );

        return Ok(result);
    }
}

/// <summary>
/// Request DTO for creating a task.
/// </summary>
public sealed record CreateTaskRequest(
    string Title,
    string? Description,
    Guid? AssignedToId,
    TaskPriority Priority,
    DateTimeOffset? DueDate
);

/// <summary>
/// Request DTO for updating task status.
/// </summary>
public sealed record UpdateMyTaskStatusRequest(TaskItemStatus Status);

public sealed record UpdateTaskStatusByAdminRequest(TaskItemStatus Status);

public sealed record UpdateTaskRequest(
    string? Title,
    string? Description,
    Guid? AssignedToId,
    TaskPriority? Priority,
    DateTimeOffset? DueDate
);
