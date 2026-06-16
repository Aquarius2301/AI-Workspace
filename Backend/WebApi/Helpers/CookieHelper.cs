namespace WebApi.Helpers;

public static class CookieHelper
{
    public static void AddCookie(HttpResponse response, string key, string value, double expiredDay)
    {
        var options = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            IsEssential = true,
            Expires = DateTimeOffset.UtcNow.AddDays(expiredDay),
        };

        response.Cookies.Append(key, value, options);
    }

    public static string? GetCookie(HttpRequest request, string key)
    {
        return request.Cookies[key];
    }

    public static void RemoveCookie(HttpResponse response, string key)
    {
        response.Cookies.Delete(key);
    }
}
