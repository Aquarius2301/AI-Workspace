using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services;

namespace WebApi.Controllers;

[ApiController]
[Route("api/upload")]
[Authorize]
public class UploadController : ControllerBase
{
    private readonly ImageKitService _imageKitService;

    public UploadController(ImageKitService imageKitService)
    {
        _imageKitService = imageKitService;
    }

    /// <summary>
    /// Upload an image file to ImageKit. Returns the CDN URL of the uploaded image.
    /// </summary>
    /// <param name="file">The image file to upload (JPEG, PNG, WEBP, GIF).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns the URL of the uploaded image.</response>
    /// <response code="400">Invalid file type or file is empty.</response>
    /// <response code="401">Unauthorized if the user is not authenticated.</response>
    /// <response code="500">InternalServerError if an upload error occurs.</response>
    [HttpPost("picture")]
    public async Task<IActionResult> UploadPicture(
        IFormFile file,
        CancellationToken cancellationToken
    )
    {
        if (file is null || file.Length == 0)
            return BadRequest("File is empty.");

        // Validate content type
        var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };
        if (!allowedTypes.Contains(file.ContentType))
            return BadRequest("Invalid file type. Allowed: JPEG, PNG, WEBP, GIF.");

        using var stream = file.OpenReadStream();
        var imageUrl = await _imageKitService.UploadImageAsync(
            stream,
            file.FileName,
            cancellationToken
        );

        return Ok(new { avatarUrl = imageUrl });
    }
}
