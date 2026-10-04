namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/v1/site-content/feature-highlights-intro/query's response shape.</summary>
public record FeatureHighlightsIntroResponse(Guid Id, string Description, Guid? CoverMediaAssetId, int DisplayOrder) : IQueryableFields
{
    public static string[] SearchFields => [nameof(Description)];
}
