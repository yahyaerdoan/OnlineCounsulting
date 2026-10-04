namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/v1/site-content/gallery-categories/query's response shape.</summary>
public record GalleryCategoryResponse(Guid Id, string Name, string? Description) : IQueryableFields
{
    public static string[] SearchFields => [nameof(Name), nameof(Description)];
}
