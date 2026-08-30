namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/site-content/social-links/query's response shape.</summary>
public record SocialLinkResponse(Guid Id, string Name, string Url, string Icon, string? IconColor, int DisplayOrder) : IQueryableFields
{
    public static string[] SearchFields => [nameof(Name)];
}
