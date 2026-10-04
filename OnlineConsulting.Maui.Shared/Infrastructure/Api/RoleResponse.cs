namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/v1/roles/query's response shape.</summary>
public record RoleResponse(Guid Id, string Name, string? Description) : IQueryableFields
{
    public static string[] SearchFields => [nameof(Name)];
}
