namespace AIWorkspace.Api.Helpers;

public static class CookieKey
{
    public const string AccessToken = "access_token";
    public const string RefreshToken = "refresh_token";
    public const string DeviceId = "device_id";
}

public static class CookieHelper
{
    /// <summary>
    /// Adds a cookie to the HTTP response with specified key, value, and expiration time.
    /// </summary>
    /// <param name="response">The HTTP response to which the cookie will be added.</param>
    /// <param name="key">The key (name) of the cookie.</param>
    /// <param name="value">The value of the cookie.</param>
    /// <param name="expiresIn">The duration after which the cookie will expire.</param>
    public static void AddCookie(
        HttpResponse response,
        string key,
        string value,
        TimeSpan expiresIn
    )
    {
        var options = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            IsEssential = true,
            Expires = DateTimeOffset.UtcNow.Add(expiresIn),
        };

        response.Cookies.Append(key, value, options);
    }

    /// <summary>
    /// Retrieves the value of a cookie from the HTTP request by its key.
    /// </summary>
    /// <param name="request">The HTTP request from which the cookie will be retrieved.</param>
    /// <param name="key">The key (name) of the cookie to retrieve.</param>
    /// <returns>The value of the cookie if found; otherwise, null.</returns>
    public static string? GetCookie(HttpRequest request, string key)
    {
        return request.Cookies[key];
    }

    /// <summary>
    /// Removes a cookie from the HTTP response by its key.
    /// </summary>
    /// <param name="response">The HTTP response from which the cookie will be removed.</param>
    /// <param name="key">The key (name) of the cookie to remove.</param>
    public static void RemoveCookie(HttpResponse response, string key)
    {
        response.Cookies.Delete(key);
    }
}
