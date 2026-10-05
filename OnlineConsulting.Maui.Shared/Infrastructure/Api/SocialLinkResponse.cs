using OnlineConsulting.Maui.Shared.Infrastructure.Hateoas;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/v1/site-content/social-links/query's response shape.</summary>
public record SocialLinkResponse(Guid Id, string Name, string Url, string Icon, string? IconColor, int DisplayOrder) : HalResource, IQueryableFields
{
    public static string[] SearchFields => [nameof(Name)];
}
