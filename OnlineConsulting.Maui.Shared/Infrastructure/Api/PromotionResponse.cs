namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/site-content/promotions/query's response shape.</summary>
public record PromotionResponse(Guid Id, string Title, string Description, string? CtaText, string? CtaUrl, DateTimeOffset? ExpiresAt, int DisplayOrder) : IQueryableFields
{
    public static string[] SearchFields => [nameof(Title), nameof(Description)];
}
