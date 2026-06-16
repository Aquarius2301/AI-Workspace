using BusinessObject.Enums;
using Infrastructure.Exceptions;
using Infrastructure.Functions.Projects.Commands;
using Infrastructure.Functions.Projects.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Helpers;

namespace WebApi.Controllers;

[ApiController]
[Route("api/projects")]
[Authorize]
public class ProjectController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// GET /api/projects/{projectId} - Lấy chi tiết thông tin và trạng thái Public/Private của một dự án.
    /// Nếu public thì chỉ team member xem, còn private thì chỉ có project member xem được.
    /// Mọi thành viên có quyền tiếp cận project, hoặc Admin/Leader của Team.
    /// </summary>
    [HttpGet("{projectId:guid}")]
    public async Task<IActionResult> GetProjectDetail(Guid projectId)
    {
        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(new GetProjectDetailQuery(userId, projectId));

        return Ok(result);
    }

    /// <summary>
    /// GET /api/projects/team/{teamId} - Lấy danh sách các dự án hiển thị được cho User.
    /// Gồm các dự án Public và dự án Private mà User là thành viên.
    /// </summary>
    [HttpGet("team/{teamId:guid}")]
    public async Task<IActionResult> GetTeamProjects(Guid teamId)
    {
        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(new GetTeamProjectsQuery(userId, teamId));

        return Ok(result);
    }

    /// <summary>
    /// GET /api/projects/team/{teamId}/all - Lấy toàn bộ không sót một dự án nào (Admin Only).
    /// </summary>
    [HttpGet("team/{teamId:guid}/all")]
    public async Task<IActionResult> GetTeamAllProjects(Guid teamId)
    {
        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(new GetTeamAllProjectsQuery(userId, teamId));

        return Ok(result);
    }

    /// <summary>
    /// POST /api/projects/team/{teamId} - Tạo một dự án mới trong Team.
    /// Người tạo tự động lưu thành CreatorId (Admin hoặc Leader của Team).
    /// </summary>
    [HttpPost("team/{teamId:guid}")]
    public async Task<IActionResult> CreateProject(
        Guid teamId,
        [FromBody] CreateProjectRequest request
    )
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new BadRequestException("Project name is required");

        var userId = ClaimHelper.GetCurrentUserId();

        var visibility =
            request.IsPublic.HasValue && request.IsPublic.Value
                ? ProjectVisibility.Public
                : ProjectVisibility.Private;

        var result = await _mediator.Send(
            new CreateProjectCommand(userId, teamId, request.Name, request.Description, visibility)
        );

        return Ok(result);
    }

    /// <summary>
    /// PUT /api/projects/{projectId} - Chỉnh sửa thông tin hoặc thay đổi chế độ Public/Private.
    /// </summary>
    [HttpPut("{projectId:guid}")]
    public async Task<IActionResult> UpdateProject(
        Guid projectId,
        [FromBody] UpdateProjectRequest request
    )
    {
        var userId = ClaimHelper.GetCurrentUserId();

        ProjectVisibility? visibility = request.IsPublic.HasValue
            ? (request.IsPublic.Value ? ProjectVisibility.Public : ProjectVisibility.Private)
            : null;

        var result = await _mediator.Send(
            new UpdateProjectCommand(
                userId,
                projectId,
                request.Name,
                request.Description,
                visibility
            )
        );

        return Ok(result);
    }

    /// <summary>
    /// DELETE /api/projects/{projectId} - Xóa vĩnh viễn dự án khỏi hệ thống.
    /// </summary>
    [HttpDelete("{projectId:guid}")]
    public async Task<IActionResult> DeleteProject(Guid projectId)
    {
        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(new DeleteProjectCommand(userId, projectId));

        return Ok("Success");
    }

    /// <summary>
    /// POST /api/projects/{projectId}/member - Mời một thành viên từ trong Team vào tham gia dự án Private.
    /// </summary>
    [HttpPost("{projectId:guid}/member")]
    public async Task<IActionResult> AddProjectMember(
        Guid projectId,
        [FromBody] AddProjectMemberRequest request
    )
    {
        if (request.UserId == Guid.Empty)
            throw new BadRequestException("UserId is required");

        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(new AddProjectMemberCommand(userId, projectId, request.UserId));

        return Ok("Success");
    }

    /// <summary>
    /// DELETE /api/projects/{projectId}/member/{userId} - Trục xuất một thành viên khỏi dự án Private.
    /// </summary>
    [HttpDelete("{projectId:guid}/member/{userId:guid}")]
    public async Task<IActionResult> RemoveProjectMember(Guid projectId, Guid userId)
    {
        var currentUserId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(new RemoveProjectMemberCommand(currentUserId, projectId, userId));

        return Ok("Success");
    }

    /// <summary>
    /// GET /api/projects/{projectId}/members - Xem danh sách tất cả các thành viên đang cùng tham gia trong dự án.
    /// </summary>
    [HttpGet("{projectId:guid}/members")]
    public async Task<IActionResult> GetProjectMembers(Guid projectId)
    {
        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(new GetProjectMembersQuery(userId, projectId));

        return Ok(result);
    }

    /// <summary>
    /// GET /api/projects/team/{teamId}/available-members - Lấy danh sách những thành viên trong Team
    /// nhưng chưa được mời vào dự án để hiển thị lên ô chọn trên giao diện.
    /// </summary>
    [HttpGet("team/{teamId:guid}/available-members")]
    public async Task<IActionResult> GetAvailableProjectMembers(
        Guid teamId,
        [FromQuery] Guid projectId
    )
    {
        if (projectId == Guid.Empty)
            throw new BadRequestException("ProjectId query parameter is required");

        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(
            new GetAvailableProjectMembersQuery(userId, teamId, projectId)
        );

        return Ok(result);
    }
}

public sealed record CreateProjectRequest(string Name, string? Description, bool? IsPublic);

public sealed record UpdateProjectRequest(string? Name, string? Description, bool? IsPublic);

public sealed record AddProjectMemberRequest(Guid UserId);
