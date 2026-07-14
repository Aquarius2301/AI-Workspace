using System.Security.Cryptography;
using System.Text;

namespace AIWorkspace.Application.Helpers;

public static class SlugHelper
{
    private const string Chars = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const int MinLength = 10;
    private const int MaxLengthExclusive = 16;

    /// <summary>
    /// Generates a cryptographically random alphanumeric slug (length 10-15).
    /// </summary>
    public static string GenerateSlug()
    {
        int length = RandomNumberGenerator.GetInt32(MinLength, MaxLengthExclusive);
        var sb = new StringBuilder(length);
        for (int i = 0; i < length; i++)
        {
            sb.Append(Chars[RandomNumberGenerator.GetInt32(Chars.Length)]);
        }
        return sb.ToString();
    }
}
