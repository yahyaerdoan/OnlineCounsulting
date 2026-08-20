using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace OnlineConsulting.SharedKernel.Slugs;

public static partial class SlugGenerator
{
    private const int MaxLength = 80;
    private const char TurkishDotlessI = 'ı';
    private const int MaxSequentialAttempts = 50;

    public static string Slugify(string value, int maxLength = MaxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var normalized = value.Replace(TurkishDotlessI, 'i').Normalize(NormalizationForm.FormKD);

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

    public static async Task<string> GenerateUniqueAsync(string value, Func<string, Task<bool>> existsAsync, int maxLength = MaxLength)
    {
        var baseSlug = Slugify(value, maxLength);
        var slug = baseSlug;

        for (var suffix = 1; suffix <= MaxSequentialAttempts; suffix++)
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
