using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Services;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Api.Backgrounds;

/// <summary>
/// Background service that periodically cleans up orphaned pictures
/// (IsActive = false) that are older than 1 day.
/// </summary>
public sealed class PictureCleanupBackground : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PictureCleanupBackground> _logger;
    private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(24);
    private readonly TimeSpan _orphanThreshold = TimeSpan.FromDays(1);

    public PictureCleanupBackground(
        IServiceProvider serviceProvider,
        ILogger<PictureCleanupBackground> logger
    )
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("PictureCleanupBackground started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupOrphanedPicturesAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Error occurred while cleaning up orphaned pictures.");
            }

            await Task.Delay(_cleanupInterval, stoppingToken);
        }

        _logger.LogInformation("PictureCleanupBackground stopped.");
    }

    private async Task CleanupOrphanedPicturesAsync(CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
        var imageKitService = scope.ServiceProvider.GetRequiredService<IImageKitService>();

        var cutoffDate = DateTimeOffset.UtcNow - _orphanThreshold;

        var orphanedPictures = await context
            .Pictures.AsNoTracking()
            .Where(p => !p.IsActive && p.CreatedAt < cutoffDate)
            .ToListAsync(ct);

        if (orphanedPictures.Count == 0)
        {
            _logger.LogDebug("No orphaned pictures to clean up.");
            return;
        }

        _logger.LogInformation("Cleaning up {Count} orphaned pictures.", orphanedPictures.Count);

        foreach (var picture in orphanedPictures)
        {
            try
            {
                await imageKitService.DeleteAsync(picture.FileId, ct);
                context.Pictures.Remove(picture);
                _logger.LogDebug(
                    "Deleted orphaned picture {PictureId} (FileId: {FileId})",
                    picture.Id,
                    picture.FileId
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to delete orphaned picture {PictureId} (FileId: {FileId})",
                    picture.Id,
                    picture.FileId
                );
            }
        }

        await context.SaveChangesAsync(ct);
    }
}
