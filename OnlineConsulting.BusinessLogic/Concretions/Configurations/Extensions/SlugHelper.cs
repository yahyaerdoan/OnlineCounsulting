using System.Text.RegularExpressions;

namespace OnlineConsulting.BusinessLogic.Concretions.Configurations.Extensions;

public static class SlugHelper
{
    public static string GenerateSlug(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        value = value.ToLowerInvariant().Trim();

        value = value
            .Replace("ğ", "g").Replace("ü", "u").Replace("ş", "s")
            .Replace("ı", "i").Replace("ö", "o").Replace("ç", "c");

        value = Regex.Replace(value, @"[^a-z0-9\s-]", "");
        value = Regex.Replace(value, @"\s+", "-");
        value = Regex.Replace(value, @"-+", "-");

        return value.Trim('-');
    }
}
