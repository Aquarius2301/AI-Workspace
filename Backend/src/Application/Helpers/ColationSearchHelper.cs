using System.Globalization;
using System.Text;

namespace AIWorkspace.Application.Helpers;

public static class CollationSearchHelper
{
    /// <summary>
    /// Removes Vietnamese diacritics and special characters, normalizing to base ASCII form.
    /// For example: "Thành phố Hồ Chí Minh" → "Thanh pho Ho Chi Minh"
    /// Also converts to lowercase for consistent comparison.
    /// </summary>
    public static string RemoveVietnameseDiacritics(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        // Normalize to Unicode Form D (NFD) to separate base characters from diacritics
        var normalized = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();

        foreach (var ch in normalized)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(ch);
            // Keep only non-spacing marks (diacritics) are removed, keep base characters
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(ch);
            }
        }

        // Convert to lowercase
        var result = sb.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();

        // Replace specific Vietnamese characters that NFD doesn't fully handle
        result = result.Replace("đ", "d").Replace("Đ", "d");

        return result;
    }

    /// <summary>
    /// Checks if a string property contains the search term,
    /// case-insensitive and Vietnamese diacritics-insensitive.
    /// </summary>
    public static bool Contains(string? value, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        if (string.IsNullOrWhiteSpace(searchTerm))
            return true;

        var normalizedValue = RemoveVietnameseDiacritics(value);
        var normalizedSearch = RemoveVietnameseDiacritics(searchTerm);

        return normalizedValue.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase);
    }
}
