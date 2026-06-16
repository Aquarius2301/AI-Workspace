using BusinessObject.Enums;
using Infrastructure.Common.Models;
using Infrastructure.Exceptions;
using Infrastructure.Functions.Tasks.Commands;
using Infrastructure.Functions.Tasks.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Helpers;

namespace WebApi.Controllers;

[ApiController]
[Authorize]
public class TaskController : ControllerBase
{
    private readonly IMediator _mediator;

    public TaskController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// GET /api/tasks/{taskId} - Lấy chi tiết thông tin của một công việc.
    /// Mọi thành viên thuộc Project chứa task đó.
    /// </summary>
    [HttpGet("api/tasks/{taskId:guid}")]
    public async Task<IActionResult> GetTaskDetail(Guid taskId)
    {
        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(new GetTaskDetailQuery(userId, taskId));

        return Ok(result);
    }

    /// <summary>
    /// GET /api/projects/{projectId}/tasks - Lấy danh sách toàn bộ công việc thuộc một dự án,
    /// hỗ trợ phân trang, lọc theo trạng thái/thành viên.
    /// Mọi thành viên thuộc Project đó, hoặc Admin của Team.
    /// </summary>
    [HttpGet("api/projects/{projectId:guid}/tasks")]
    public async Task<IActionResult> GetProjectTasks(
        Guid projectId,
        [FromQuery] string? status,
        [FromQuery] Guid? memberId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20
    )
    {
        var userId = ClaimHelper.GetCurrentUserId();

        TaskItemStatus? taskStatus = null;
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<TaskItemStatus>(status, true, out var parsedStatus))
                throw new BadRequestException($"Invalid status value: {status}");

            taskStatus = parsedStatus;
        }

        var result = await _mediator.Send(
            new GetProjectTasksQuery(
                userId,
                projectId,
                taskStatus,
                memberId,
                new PaginationRequest(page, pageSize)
            )
        );

        return Ok(result);
    }

    /// <summary>
    /// GET /api/teams/{teamId}/tasks/me - Lấy danh sách tất cả các task được giao cho
    /// chính User đang đăng nhập trong toàn Team.
    /// Mọi thành viên thuộc Team.
    /// </summary>
    [HttpGet("api/teams/{teamId:guid}/tasks/me")]
    public async Task<IActionResult> GetMyTeamTasks(Guid teamId)
    {
        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(new GetMyTeamTasksQuery(userId, teamId));

        return Ok(result);
    }

    /// <summary>
    /// POST /api/projects/{projectId}/tasks - Tạo một công việc mới trong dự án.
    /// Admin, Leader sở hữu Project.
    /// </summary>
    [HttpPost("api/projects/{projectId:guid}/tasks")]
    public async Task<IActionResult> CreateTask(
        Guid projectId,
        [FromBody] CreateTaskRequest request
    )
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new BadRequestException("Task title is required");

        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(
            new CreateTaskCommand(
                userId,
                projectId,
                request.Title,
                request.Description,
                request.AssignedToId,
                request.Priority,
                request.DueDate
            )
        );

        return Ok(result);
    }

    /// <summary>
    /// PUT /api/tasks/{taskId} - Chỉnh sửa thông tin chi tiết của task như tiêu đề, mô tả, ngày hết hạn.
    /// Admin, Leader sở hữu Project.
    /// </summary>
    [HttpPut("api/tasks/{taskId:guid}")]
    public async Task<IActionResult> UpdateTask(Guid taskId, [FromBody] UpdateTaskRequest request)
    {
        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(
            new UpdateTaskCommand(
                userId,
                taskId,
                request.Title,
                request.Description,
                request.DueDate
            )
        );

        return Ok("Success");
    }

    /// <summary>
    /// PATCH /api/tasks/{taskId}/status - Thay đổi nhanh trạng thái công việc.
    /// Admin, Leader sở hữu project, hoặc chính Người được giao task - Assignee.
    /// </summary>
    [HttpPatch("api/tasks/{taskId:guid}/status")]
    public async Task<IActionResult> UpdateTaskStatus(
        Guid taskId,
        [FromBody] UpdateTaskStatusRequest request
    )
    {
        if (string.IsNullOrWhiteSpace(request.Status))
            throw new BadRequestException("Status is required");

        if (!Enum.TryParse<TaskItemStatus>(request.Status, true, out var status))
            throw new BadRequestException($"Invalid status value: {request.Status}");

        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(new UpdateTaskStatusCommand(userId, taskId, status));

        return Ok("Success");
    }

    /// <summary>
    /// PATCH /api/tasks/{taskId}/assign - Giao task hoặc đổi người thực hiện sang một thành viên khác trong cùng project.
    /// Admin, Leader sở hữu Project.
    /// </summary>
    [HttpPatch("api/tasks/{taskId:guid}/assign")]
    public async Task<IActionResult> AssignTask(Guid taskId, [FromBody] AssignTaskRequest request)
    {
        if (request.AssignedToId == Guid.Empty)
            throw new BadRequestException("AssignedToId is required");

        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(new AssignTaskCommand(userId, taskId, request.AssignedToId));

        return Ok("Success");
    }

    /// <summary>
    /// DELETE /api/tasks/{taskId} - Xóa vĩnh viễn công việc khỏi dự án.
    /// Admin, Leader sở hữu Project.
    /// </summary>
    [HttpDelete("api/tasks/{taskId:guid}")]
    public async Task<IActionResult> DeleteTask(Guid taskId)
    {
        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(new DeleteTaskCommand(userId, taskId));

        return Ok("Success");
    }
}

public sealed record CreateTaskRequest(
    string Title,
    string? Description,
    Guid? AssignedToId,
    int Priority,
    DateTime? DueDate
);

public sealed record UpdateTaskRequest(string? Title, string? Description, DateTime? DueDate);

public sealed record UpdateTaskStatusRequest(string Status);

public sealed record AssignTaskRequest(Guid AssignedToId);
