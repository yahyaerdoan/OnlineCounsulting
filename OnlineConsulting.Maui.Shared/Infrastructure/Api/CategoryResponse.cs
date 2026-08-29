namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/categories/query's response shape.</summary>
public record CategoryResponse(Guid Id, string Title, string Description, string Icon, string? IconColor) : IQueryableFields
{
    public static string[] SearchFields => [nameof(Title), nameof(Description)];
}
