namespace AIWorkspace.Application.Common.Services;

public interface IJwtService
{
    /// <summary>
    /// Generates a JWT token for the specified user ID and email.
    /// </summary>
    /// <param name="userId">The ID of the user for whom to generate the token.</param>
    /// <param name="email">The email of the user for whom to generate the token.</param>
    /// <param name="deviceId">The device identifier to embed in the token.</param>
    /// <returns>The generated JWT token.</returns>
    public string GenerateAccessToken(Guid userId, string email, string deviceId);

    /// <summary>
    /// Generates a refresh token for the specified user ID and email.
    /// </summary>
    /// <returns>The generated refresh token.</returns>
    public string GenerateRefreshToken();
}
