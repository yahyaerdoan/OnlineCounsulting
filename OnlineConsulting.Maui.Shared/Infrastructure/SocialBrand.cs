using OnlineConsulting.Maui.Shared.Infrastructure.Api;

namespace OnlineConsulting.Maui.Shared.Infrastructure;

/// <summary>Colors a social link in its platform's own brand color (app.css "brand-*"), recognized from the URL or the name;
/// an unknown platform falls back to the admin-picked IconColor.</summary>
public static class SocialBrand
{
    private static readonly (string Key, string[] Hints)[] Brands =
    [
        ("instagram", ["instagram"]),
        ("facebook", ["facebook", "fb.com", "fb.me"]),
        ("youtube", ["youtube", "youtu.be"]),
        ("linkedin", ["linkedin"]),
        ("tiktok", ["tiktok"]),
        ("pinterest", ["pinterest"]),
        ("whatsapp", ["whatsapp", "wa.me"]),
        ("yelp", ["yelp"]),
        ("nextdoor", ["nextdoor"]),
        ("google", ["google", "g.page"]),
        ("x", ["twitter", "x.com"]),
        ("threads", ["threads.net"]),
    ];

    /// <summary>"brand-instagram" etc., or null when the platform isn't recognized.</summary>
    public static string? ClassFor(SocialLinkResponse link) => ClassFor(link.Url, link.Name);

    /// <summary>Inline --brand for an unrecognized platform with an admin-picked color, otherwise null.</summary>
    public static string? StyleFor(SocialLinkResponse link) => StyleFor(link.Url, link.Name, link.IconColor);

    public static string? ClassFor(PartnershipSocialLinkResponse link) => ClassFor(link.Url, link.Name);

    public static string? StyleFor(PartnershipSocialLinkResponse link) => StyleFor(link.Url, link.Name, link.IconColor);

    private static string? ClassFor(string url, string name)
    {
        var haystack = $"{url} {name}".ToLowerInvariant();
        var brand = Brands.FirstOrDefault(b => b.Hints.Any(haystack.Contains)).Key;
        return brand is null ? null : $"brand-{brand}";
    }

    private static string? StyleFor(string url, string name, string? iconColor) =>
        ClassFor(url, name) is null && !string.IsNullOrWhiteSpace(iconColor) ? $"--brand:{iconColor};" : null;
}
