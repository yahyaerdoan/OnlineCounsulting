namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/site-content/hero-slides/query's response shape.</summary>
public record HeroSlideResponse(Guid Id, string Title, string Description, string ImageUrl, int DisplayOrder) : IQueryableFields
{
    public static string[] SearchFields => [nameof(Title), nameof(Description)];
}
