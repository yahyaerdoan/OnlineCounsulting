namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/site-content/about-us/query's response shape.</summary>
public record AboutUsResponse(Guid Id, string Title, string Description, string? CoverImage, string? VideoUrl, int DisplayOrder) : IQueryableFields
{
    public static string[] SearchFields => [nameof(Title), nameof(Description)];
}
