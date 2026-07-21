using AIWorkspace.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Api.Backgrounds;

public sealed class RefreshTokenCleanupBackground : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RefreshTokenCleanupBackground> _logger;
    private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(1);

    public RefreshTokenCleanupBackground(
        IServiceProvider serviceProvider,
        ILogger<RefreshTokenCleanupBackground> logger
    )
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RefreshTokenCleanupBackground started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupExpiredTokensAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogWarning(ex, "Failed to clean up expired refresh tokens");
            }

            await Task.Delay(_cleanupInterval, stoppingToken);
        }

        _logger.LogInformation("RefreshTokenCleanupBackground stopped.");
    }

    private async Task CleanupExpiredTokensAsync(CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

        // Bulk delete expired tokens directly at DB level
        var deletedCount = await context
            .RefreshTokens.Where(rt => rt.ExpiresAt < DateTimeOffset.UtcNow)
            .ExecuteDeleteAsync(ct);

        if (deletedCount > 0)
        {
            _logger.LogInformation("Cleaned up {Count} expired refresh tokens", deletedCount);
        }
    }
}
