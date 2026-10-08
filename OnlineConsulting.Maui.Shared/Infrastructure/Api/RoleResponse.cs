using OnlineConsulting.Maui.Shared.Infrastructure.Hateoas;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/v1/roles/query's response shape.</summary>
public record RoleResponse(Guid Id, string Name, string? Description) : HalResource, IQueryableFields
{
    public static string[] SearchFields => [nameof(Name)];
}
