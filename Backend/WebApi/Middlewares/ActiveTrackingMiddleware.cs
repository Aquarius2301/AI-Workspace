using System.Collections.Concurrent;
using DataAccess.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using WebApi.Helpers;

namespace WebApi.Middlewares;

/// <summary>
/// Middleware for tracking user activity by updating the LastActiveAt timestamp.
/// </summary>
public class ActiveTrackingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ActiveTrackingMiddleware> _logger;

    // Cache (userId → lastActive timestamp) avoid spamming DB
    private static readonly ConcurrentDictionary<Guid, DateTimeOffset> ActiveCache = new();
    private static readonly TimeSpan ThrottleInterval = TimeSpan.FromMinutes(10);

    // Endpoints don't need tracking active
    private static readonly HashSet<string> ExcludedPaths = new(StringComparer.OrdinalIgnoreCase)
    {
        "/api/auth/login",
        "/api/auth/register",
        "/api/auth/refresh",
    };

    public ActiveTrackingMiddleware(RequestDelegate next, ILogger<ActiveTrackingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IUnitOfWork unitOfWork)
    {
        // Check if the user is authenticated before tracking
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var path = context.Request.Path.Value ?? string.Empty;

            // Skip tracking for excluded paths
            if (!IsExcluded(path))
            {
                await TrackActiveAsync(context, unitOfWork);
            }
        }

        await _next(context);
    }

    private static bool IsExcluded(string path)
    {
        foreach (var excluded in ExcludedPaths)
        {
            if (path.Contains(excluded, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    private async Task TrackActiveAsync(HttpContext context, IUnitOfWork unitOfWork)
    {
        var userId = ClaimHelper.GetCurrentUserId();
        if (userId == Guid.Empty)
            return;

        var now = DateTimeOffset.UtcNow;

        // Check cache first, skip if the last update was within the throttle interval
        if (ActiveCache.TryGetValue(userId, out var lastActive))
        {
            if (now - lastActive < ThrottleInterval)
                return;
        }

        try
        {
            // Update LastActiveAt trong DB
            var user = await unitOfWork.Users.GetQuery().FirstOrDefaultAsync(u => u.Id == userId);

            if (user != null)
            {
                user.LastActiveAt = now;
                await unitOfWork.SaveChangesAsync();
                ActiveCache[userId] = now;

                _logger.LogDebug("Updated LastActiveAt for user {UserId}", userId);
            }
        }
        catch (Exception ex)
        {
            // No throw, just log the error. We don't want to break the request if tracking fails.
            _logger.LogWarning(ex, "Failed to update LastActiveAt for user {UserId}", userId);
        }
    }
}
