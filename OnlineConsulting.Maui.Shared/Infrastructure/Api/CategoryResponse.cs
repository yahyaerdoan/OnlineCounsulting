using OnlineConsulting.Maui.Shared.Infrastructure.Hateoas;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/v1/categories/query's response shape.</summary>
public record CategoryResponse(Guid Id, string Title, string Description, string Icon, string? IconColor) : HalResource, IQueryableFields
{
    public static string[] SearchFields => [nameof(Title), nameof(Description)];
}
