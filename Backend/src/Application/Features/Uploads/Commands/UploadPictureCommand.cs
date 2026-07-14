using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Services;
using AIWorkspace.Domain.Entities;
using MediatR;

namespace AIWorkspace.Application.Features.Uploads;

public sealed record UploadPictureCommand(
    Guid CurrentUserId,
    Stream FileStream,
    string FileName,
    CancellationToken CancellationToken
) : IRequest<UploadPictureResult>;

public sealed record UploadPictureResult(string FileId, string Url);

public sealed class UploadPictureCommandHandler
    : IRequestHandler<UploadPictureCommand, UploadPictureResult>
{
    private readonly IAppDbContext _context;
    private readonly IImageKitService _imageKitService;

    public UploadPictureCommandHandler(IAppDbContext context, IImageKitService imageKitService)
    {
        _context = context;
        _imageKitService = imageKitService;
    }

    public async Task<UploadPictureResult> Handle(
        UploadPictureCommand request,
        CancellationToken cancellationToken
    )
    {
        // Upload to ImageKit
        var uploadResult = await _imageKitService.UploadAsync(
            request.FileStream,
            request.FileName,
            cancellationToken
        );

        // Save to database
        var picture = new Picture
        {
            Id = Guid.NewGuid(),
            FileId = uploadResult.FileId,
            Url = uploadResult.Url,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            // UsedByUserId = request.CurrentUserId,
        };

        _context.Pictures.Add(picture);
        await _context.SaveChangesAsync(cancellationToken);

        return new UploadPictureResult(picture.FileId, picture.Url);
    }
}
