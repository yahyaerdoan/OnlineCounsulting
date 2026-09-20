using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace OnlineConsulting.SharedKernel.Slugs;

public static partial class SlugGenerator
{
    private const int _maxLength = 80;
    // Normalize before the generic diacritic strip below - 'ı' has no accent to strip and would otherwise survive as-is instead of folding to 'i'.
    private const char _turkishDotlessI = 'ı';
    private const int _maxSequentialAttempts = 50;

    public static string Slugify(string value, int maxLength = _maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var normalized = value.Replace(_turkishDotlessI, 'i').Normalize(NormalizationForm.FormKD);

        var withoutDiacritics = new StringBuilder(normalized.Length);

        foreach (var ch in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
            {
                _ = withoutDiacritics.Append(ch);
            }
        }

        var slug = NonAlphanumeric().Replace(withoutDiacritics.ToString().ToLowerInvariant(), "-").Trim('-');

        return Truncate(slug, maxLength);
    }

    /// <summary>Appends -1, -2, ... on collision; past MaxSequentialAttempts falls back to a guid suffix instead of looping forever.</summary>
    public static async Task<string> GenerateUniqueAsync(string value, Func<string, Task<bool>> existsAsync, int maxLength = _maxLength)
    {
        var baseSlug = Slugify(value, maxLength);
        var slug = baseSlug;

        for (var suffix = 1; suffix <= _maxSequentialAttempts; suffix++)
        {
            if (!await existsAsync(slug))
            {
                return slug;
            }

            slug = $"{baseSlug}-{suffix}";
        }

        return await existsAsync(slug)
            ? Truncate($"{baseSlug}-{Guid.NewGuid():N}", maxLength)
            : slug;
    }

    private static string Truncate(string slug, int maxLength)
    {
        if (slug.Length <= maxLength)
        {
            return slug;
        }

        var truncated = slug[..maxLength];
        var lastDash = truncated.LastIndexOf('-');

        return lastDash > maxLength / 2 ? truncated[..lastDash] : truncated.TrimEnd('-');
    }

    [GeneratedRegex(@"[^a-z0-9]+")]
    private static partial Regex NonAlphanumeric();
}
