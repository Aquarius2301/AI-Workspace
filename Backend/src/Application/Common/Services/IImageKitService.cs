namespace AIWorkspace.Application.Common.Services;

public sealed record ImageKitUploadResult(string FileId, string Url);

public interface IImageKitService
{
    /// <summary>
    /// Uploads a file to ImageKit.IO.
    /// Maximum file size: 5 MB.
    /// </summary>
    /// <param name="fileStream">The stream of the file to upload.</param>
    /// <param name="fileName">The name of the file (with extension).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An <see cref="ImageKitUploadResult"/> containing the file ID and URL.</returns>
    Task<ImageKitUploadResult> UploadAsync(
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Deletes a file from ImageKit.IO by its file ID.
    /// </summary>
    /// <param name="fileId">The ID of the file to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAsync(string fileId, CancellationToken cancellationToken = default);
}
