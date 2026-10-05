using OnlineConsulting.Maui.Shared.Infrastructure.Hateoas;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/v1/site-content/gallery-categories/query's response shape.</summary>
public record GalleryCategoryResponse(Guid Id, string Name, string? Description) : HalResource, IQueryableFields
{
    public static string[] SearchFields => [nameof(Name), nameof(Description)];
}
