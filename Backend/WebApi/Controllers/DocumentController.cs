using BusinessObject.Enums;
using Infrastructure.Common.Models;
using Infrastructure.Exceptions;
using Infrastructure.Functions.Documents.Commands;
using Infrastructure.Functions.Documents.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Helpers;

namespace WebApi.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class DocumentController : ControllerBase
{
    private readonly IMediator _mediator;

    public DocumentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// GET /api/projects/{projectId}/documents - Lấy danh sách toàn bộ tài liệu thuộc một dự án,
    /// hỗ trợ phân trang và tìm kiếm theo tiêu đề.
    /// Mọi thành viên có quyền tiếp cận Project, hoặc Admin/Leader của Team.
    /// </summary>
    [HttpGet("projects/{projectId:guid}/documents")]
    public async Task<IActionResult> GetProjectDocuments(
        Guid projectId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null
    )
    {
        var userId = ClaimHelper.GetCurrentUserId();
        var pagination = new PaginationRequest(page, pageSize);

        var result = await _mediator.Send(
            new GetProjectDocumentsQuery(userId, projectId, pagination, search)
        );

        return Ok(result);
    }

    /// <summary>
    /// GET /api/documents/{documentId} - Xem chi tiết nội dung của một tài liệu cụ thể.
    /// Mọi thành viên có quyền tiếp cận Project chứa tài liệu đó.
    /// </summary>
    [HttpGet("documents/{documentId:guid}")]
    public async Task<IActionResult> GetDocumentDetail(Guid documentId)
    {
        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(new GetDocumentDetailQuery(userId, documentId));

        return Ok(result);
    }

    /// <summary>
    /// POST /api/projects/{projectId}/documents - Tạo một tài liệu mới trong dự án.
    /// Admin, Leader sở hữu Project, hoặc các Member có quyền viết tài liệu.
    /// </summary>
    [HttpPost("projects/{projectId:guid}/documents")]
    public async Task<IActionResult> CreateDocument(
        Guid projectId,
        [FromBody] CreateDocumentRequest request
    )
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new BadRequestException("Title is required");

        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(
            new CreateDocumentCommand(userId, projectId, request.Title, request.Content)
        );

        return Ok("Success");
    }

    /// <summary>
    /// PUT /api/documents/{documentId} - Cập nhật nội dung hoặc tiêu đề của tài liệu.
    /// Admin, Leader sở hữu Project, hoặc chính Người tạo ra tài liệu (CreatorId).
    /// </summary>
    [HttpPut("documents/{documentId:guid}")]
    public async Task<IActionResult> UpdateDocument(
        Guid documentId,
        [FromBody] UpdateDocumentRequest request
    )
    {
        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(
            new UpdateDocumentCommand(userId, documentId, request.Title, request.Content)
        );

        return Ok("Success");
    }

    /// <summary>
    /// DELETE /api/documents/{documentId} - Xóa vĩnh viễn tài liệu.
    /// Admin, Leader sở hữu Project, hoặc chính Người tạo ra tài liệu.
    /// </summary>
    [HttpDelete("documents/{documentId:guid}")]
    public async Task<IActionResult> DeleteDocument(Guid documentId)
    {
        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(new DeleteDocumentCommand(userId, documentId));

        return Ok("Success");
    }
}

// Request DTOs
public sealed record CreateDocumentRequest(string Title, string? Content);

public sealed record UpdateDocumentRequest(string? Title, string? Content);
