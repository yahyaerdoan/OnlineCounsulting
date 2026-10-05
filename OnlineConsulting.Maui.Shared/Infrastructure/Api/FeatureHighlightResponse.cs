using OnlineConsulting.Maui.Shared.Infrastructure.Hateoas;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/v1/site-content/feature-highlights/query's response shape.</summary>
public record FeatureHighlightResponse(Guid Id, string Title, string Description, string ImageUrl, int DisplayOrder) : HalResource, IQueryableFields
{
    public static string[] SearchFields => [nameof(Title), nameof(Description)];
}
