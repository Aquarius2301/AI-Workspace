namespace AIWorkspace.Api.Helpers;

public static class ClaimHelper
{
    private static IHttpContextAccessor _httpContextAccessor = null!;

    public static void Configure(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Gets the ID of the currently authenticated user.
    /// </summary>
    /// <returns>
    /// The authenticated user's ID if available and valid; otherwise, <see cref="Guid.Empty"/>.
    /// </returns>
    public static Guid GetCurrentUserId()
    {
        var user = _httpContextAccessor?.HttpContext?.User;

        return
            user?.FindFirst("sub")?.Value is string userIdStr
            && Guid.TryParse(userIdStr, out var userId)
            ? userId
            : Guid.Empty;
    }
}
