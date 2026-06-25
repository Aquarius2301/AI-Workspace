using System.Security.Claims;

namespace WebApi.Helpers;

public static class ClaimHelper
{
    private static IHttpContextAccessor _httpContextAccessor = null!;

    // Khởi tạo static để inject HttpContextAccessor (Sẽ gọi ở Program.cs)
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
            : Guid.Empty; // Trả về Guid.Empty nếu không tìm thấy hoặc không hợp lệ
    }
}
