using OnlineConsulting.Maui.Shared.Infrastructure.Hateoas;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/v1/site-content/about-us/query's response shape.</summary>
public record AboutUsResponse(Guid Id, string Title, string Description, string? CoverImage, string? VideoUrl, int DisplayOrder) : HalResource, IQueryableFields
{
    public static string[] SearchFields => [nameof(Title), nameof(Description)];
}
