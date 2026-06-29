namespace WebApi.Helpers;

public static class ClaimHelper
{
    private static IHttpContextAccessor _httpContextAccessor = null!;

    public static void Configure(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

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
