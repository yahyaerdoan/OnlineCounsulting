using OnlineConsulting.Maui.Shared.Infrastructure.Hateoas;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/v1/site-content/service-areas/query's response shape.</summary>
public record ServiceAreaResponse(Guid Id, string Name, string State, string Slug, string? IntroText, int DisplayOrder, double? Latitude = null, double? Longitude = null) : HalResource, IQueryableFields
{
    public static string[] SearchFields => [nameof(Name), nameof(State)];
}
