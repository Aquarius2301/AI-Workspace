using AIWorkspace.Api.Helpers;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Application.Features.Uploads;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIWorkspace.Api.Controllers;

[ApiController]
[Route("api/uploads")]
[Authorize]
public class UploadController : ControllerBase
{
    private readonly IMediator _mediator;

    public UploadController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Uploads a picture file to ImageKit and stores the reference in the database.
    /// </summary>
    /// <remarks>
    /// This action requires authentication. The current user's ID is extracted from the JWT claims.
    /// The file is uploaded to ImageKit CDN and the resulting FileId and Url are saved to the database.
    /// The uploaded picture is marked as active and linked to the current user.
    /// </remarks>
    /// <param name="file">The image file to upload (multipart/form-data).</param>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <response code="200">Returns the uploaded picture's FileId and Url (UploadPictureResult).</response>
    /// <response code="400">Validation failed: file is required (FileRequired, FileSizeTooLarge).</response>
    /// <response code="401">User is not authenticated (Unauthorized).</response>
    /// <response code="500">An unexpected internal server error occurred, or ImageKit upload failed (InternalServerError, ImageKitUploadFailed).</response>
    [HttpPost("picture")]
    public async Task<IActionResult> UploadPicture(
        IFormFile file,
        CancellationToken cancellationToken
    )
    {
        var userId = ClaimHelper.GetCurrentUserId();

        // Validate file early before any processing
        if (file == null || file.Length == 0)
            throw new BadRequestException(ErrorCodes.FileRequired);

        if (file.Length > 5 * 1024 * 1024) // 5 MB
            throw new BadRequestException(ErrorCodes.FileSizeTooLarge, new { MaxSizeMB = 5 });

        var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
        if (!allowedContentTypes.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase))
            throw new BadRequestException(
                ErrorCodes.BadRequest,
                new { Message = "Only image files (JPEG, PNG, GIF, WebP) are allowed." }
            );

        using var stream = file.OpenReadStream();

        var result = await _mediator.Send(
            new UploadPictureCommand(userId, stream, file.FileName, cancellationToken)
        );

        return Ok(result);
    }
}
