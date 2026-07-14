using Microsoft.AspNetCore.Identity;

namespace AIWorkspace.Application.Helpers;

public static class PasswordHasherHelper
{
    private static readonly PasswordHasher<object> _hasher = new PasswordHasher<object>();

    private static readonly object _dummyUser = new object();

    /// <summary>
    /// Hashes a plain-text password using ASP.NET Core Identity's password hasher.
    /// </summary>
    /// <param name="password">
    /// The plain-text password to hash.
    /// </param>
    /// <returns>
    /// The hashed password.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="password"/> is null or empty.
    /// </exception>
    public static string Hash(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentNullException(nameof(password));

        return _hasher.HashPassword(_dummyUser, password);
    }

    /// <summary>
    /// Verifies whether a plain-text password matches a hashed password.
    /// </summary>
    /// <param name="hashedPassword">
    /// The previously hashed password.
    /// </param>
    /// <param name="providedPassword">
    /// The plain-text password to verify.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the password is valid; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool Verify(string hashedPassword, string providedPassword)
    {
        var result = _hasher.VerifyHashedPassword(_dummyUser, hashedPassword, providedPassword);

        return result switch
        {
            PasswordVerificationResult.Failed => false,
            PasswordVerificationResult.Success => true,
            PasswordVerificationResult.SuccessRehashNeeded => true,
            _ => false,
        };
    }
}
