namespace BusinessObject.Entities;

public class RefreshToken
{
    /// <summary>
    /// Gets or sets the unique identifier of the refresh token.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the associated user.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the user who owns this refresh token.
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the refresh token value.
    /// </summary>
    public string Token { get; set; } = null!;

    /// <summary>
    /// Gets or sets the date and time when the token was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the expiration date and time of the token.
    /// </summary>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets the optional device information for the token.
    /// </summary>
    public string? DeviceInfo { get; set; }

    /// <summary>
    /// Gets or sets the persistent device identifier (UUID cookie) that uniquely
    /// identifies the browser/device that created this token.
    /// </summary>
    public string? DeviceId { get; set; }
}
