using OnlineConsulting.Maui.Shared.Infrastructure.Hateoas;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/v1/site-content/service-offerings/query's response shape.</summary>
public record ServiceOfferingResponse(Guid Id, string Title, string Description, string Icon, string? IconColor, int DisplayOrder) : HalResource, IQueryableFields
{
    public static string[] SearchFields => [nameof(Title), nameof(Description)];
}
