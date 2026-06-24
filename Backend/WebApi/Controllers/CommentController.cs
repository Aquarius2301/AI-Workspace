using BusinessObject.Enums;
using Infrastructure.Common.Models;
using Infrastructure.Exceptions;
using Infrastructure.Functions.Comments.Commands;
using Infrastructure.Functions.Comments.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Helpers;

namespace WebApi.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class CommentController : ControllerBase
{
    private readonly IMediator _mediator;

    public CommentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// GET /api/tasks/{taskId}/comments - Lấy danh sách tất cả bình luận thuộc một Task,
    /// xếp theo thứ tự thời gian từ cũ đến mới.
    /// Mọi thành viên có quyền tiếp cận Project chứa task.
    /// </summary>
    [HttpGet("tasks/{taskId:guid}/comments")]
    public async Task<IActionResult> GetTaskComments(Guid taskId)
    {
        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(new GetTaskCommentsQuery(userId, taskId));

        return Ok(result);
    }

    /// <summary>
    /// POST /api/tasks/{taskId}/comments - Gửi một bình luận mới vào Task.
    /// Mọi thành viên có quyền tiếp cận Project chứa task.
    /// </summary>
    [HttpPost("tasks/{taskId:guid}/comments")]
    public async Task<IActionResult> CreateTaskComment(
        Guid taskId,
        [FromBody] CreateCommentRequest request
    )
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            throw new BadRequestException("Content is required");

        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(new CreateTaskCommentCommand(userId, taskId, request.Content));

        return Ok("Success");
    }

    /// <summary>
    /// GET /api/documents/{documentId}/comments - Lấy danh sách tất cả bình luận nằm trong một tài liệu.
    /// Mọi thành viên có quyền tiếp cận Project chứa tài liệu.
    /// </summary>
    [HttpGet("documents/{documentId:guid}/comments")]
    public async Task<IActionResult> GetDocumentComments(Guid documentId)
    {
        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(new GetDocumentCommentsQuery(userId, documentId));

        return Ok(result);
    }

    /// <summary>
    /// POST /api/documents/{documentId}/comments - Gửi một bình luận mới vào tài liệu.
    /// Mọi thành viên có quyền tiếp cận Project chứa tài liệu.
    /// </summary>
    [HttpPost("documents/{documentId:guid}/comments")]
    public async Task<IActionResult> CreateDocumentComment(
        Guid documentId,
        [FromBody] CreateCommentRequest request
    )
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            throw new BadRequestException("Content is required");

        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(new CreateDocumentCommentCommand(userId, documentId, request.Content));

        return Ok("Success");
    }

    /// <summary>
    /// PUT /api/comments/{commentId} - Chỉnh sửa nội dung bình luận đã gửi.
    /// Chỉ chính chủ sở hữu (CreatorId) của bình luận đó mới có quyền sửa.
    /// </summary>
    [HttpPut("comments/{commentId:guid}")]
    public async Task<IActionResult> UpdateComment(
        Guid commentId,
        [FromBody] UpdateCommentContentRequest request
    )
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            throw new BadRequestException("Content is required");

        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(new UpdateCommentCommand(userId, commentId, request.Content));

        return Ok("Success");
    }

    /// <summary>
    /// DELETE /api/comments/{commentId} - Xóa một bình luận.
    /// Admin, Leader sở hữu Project chứa bình luận, hoặc chính chủ sở hữu của bình luận đó.
    /// </summary>
    [HttpDelete("comments/{commentId:guid}")]
    public async Task<IActionResult> DeleteComment(Guid commentId)
    {
        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(new DeleteCommentCommand(userId, commentId));

        return Ok("Success");
    }
}

// Request DTOs
public sealed record CreateCommentRequest(string Content);

public sealed record UpdateCommentContentRequest(string Content);
