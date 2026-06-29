namespace Infrastructure.Settings;

/// <summary>
/// Represents authentication settings used to generate tokens.
/// </summary>
public sealed class AuthSetting
{
    /// <summary>
    /// Gets or sets the JWT signing key or token secret.
    /// </summary>
    public string JwtKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the token issuer.
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the token audience.
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the access token lifetime in minutes.
    /// </summary>
    public int AccessTokenMinutes { get; set; }

    /// <summary>
    /// Gets or sets the refresh token lifetime in days.
    /// </summary>
    public int RefreshTokenDays { get; set; }
}
