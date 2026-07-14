using System.Collections.Concurrent;
using AIWorkspace.Api.Helpers;
using AIWorkspace.Application.Common;
using AIWorkspace.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AIWorkspace.Api.Middlewares;

/// <summary>
/// Middleware for tracking user activity by updating the LastActiveAt timestamp.
/// Uses in-memory cache to throttle DB writes.
/// </summary>
public class ActiveTrackingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ActiveTrackingMiddleware> _logger;
    private readonly TimeSpan _throttleInterval;

    // Cache (userId → lastActive timestamp) to avoid spamming DB
    // Uses bounded capacity to prevent memory leak from inactive users
    private static readonly ConcurrentDictionary<Guid, DateTimeOffset> ActiveCache = new(
        concurrencyLevel: Environment.ProcessorCount,
        capacity: 100_000 // Max ~100K tracked users before eviction starts
    );

    // Endpoints that don't need active tracking (exact path match for safety)
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
        // Throttle interval: update LastActiveAt at most once every 10 minutes per user
        // to avoid spamming the database on every authenticated request.
        _throttleInterval = TimeSpan.FromMinutes(10);
    }

    public async Task InvokeAsync(HttpContext context, IAppDbContext dbContext)
    {
        // Check if the user is authenticated before tracking
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var path = context.Request.Path.Value ?? string.Empty;

            // Skip tracking for excluded paths using exact match
            if (!IsExcluded(path))
            {
                await TrackActiveAsync(dbContext);
            }
        }

        await _next(context);
    }

    private static bool IsExcluded(string path)
    {
        // Use exact path matching instead of Contains to avoid false positives
        // e.g. "/api/auth/login/custom" should not match "/api/auth/login"
        return ExcludedPaths.Contains(path);
    }

    private async Task TrackActiveAsync(IAppDbContext dbContext)
    {
        var userId = ClaimHelper.GetCurrentUserId();
        if (userId == Guid.Empty)
            return;

        var now = DateTimeOffset.UtcNow;

        // Check cache first, skip if the last update was within the throttle interval
        // This is a hot path - avoids DB round-trip for most requests
        if (ActiveCache.TryGetValue(userId, out var lastActive))
        {
            if (now - lastActive < _throttleInterval)
                return;
        }

        try
        {
            // Use raw SQL update to avoid loading the full entity into memory
            var rowsAffected = await dbContext
                .Users.Where(u => u.Id == userId)
                .ExecuteUpdateAsync(u => u.SetProperty(x => x.LastActiveAt, now));

            if (rowsAffected > 0)
            {
                // Update cache only on successful DB write
                ActiveCache[userId] = now;
                _logger.LogDebug("Updated LastActiveAt for user {UserId}", userId);
            }
        }
        catch (Exception ex)
        {
            // Don't throw - we don't want to break the request if tracking fails
            _logger.LogWarning(ex, "Failed to update LastActiveAt for user {UserId}", userId);
        }
    }
}
