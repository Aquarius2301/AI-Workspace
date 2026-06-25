using DataAccess.UnitOfWork;

namespace WebApi.Middlewares;

public class RefreshTokenCleanupMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RefreshTokenCleanupMiddleware> _logger;

    // Run cleanup every 1 hour
    private static readonly TimeSpan CleanupInterval = TimeSpan.FromHours(1);
    private static DateTime _lastCleanup = DateTime.MinValue;

    public RefreshTokenCleanupMiddleware(
        RequestDelegate next,
        ILogger<RefreshTokenCleanupMiddleware> logger
    )
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IUnitOfWork unitOfWork)
    {
        // Check if cleanup is needed
        if (DateTime.UtcNow - _lastCleanup >= CleanupInterval)
        {
            try
            {
                var expiredTokens = unitOfWork
                    .RefreshTokens.GetQuery()
                    .Where(rt => rt.ExpiresAt < DateTime.UtcNow)
                    .ToList();

                if (expiredTokens.Count > 0)
                {
                    unitOfWork.RefreshTokens.RemoveRange(expiredTokens);
                    await unitOfWork.SaveChangesAsync();
                    _logger.LogInformation(
                        "Cleaned up {Count} expired refresh tokens",
                        expiredTokens.Count
                    );
                }

                _lastCleanup = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to clean up expired refresh tokens");
            }
        }

        await _next(context);
    }
}
