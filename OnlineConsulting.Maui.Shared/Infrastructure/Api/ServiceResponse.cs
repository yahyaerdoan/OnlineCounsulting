namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/services/query's response shape (MediaItems always empty on list queries).</summary>
public record ServiceResponse(
    Guid Id,
    Guid CategoryId,
    string Title,
    string Slug,
    string Description,
    string DetailedDescription,
    decimal Price,
    string PriceType,
    decimal? PriceMax,
    bool FeaturedArea,
    int DiscountRate,
    int TaxRate,
    decimal DiscountedPrice,
    bool RequiresPrepayment,
    bool IsEmergencyAvailable,
    Guid? CoverMediaAssetId) : IQueryableFields
{
    public static string[] SearchFields => [nameof(Title), nameof(Description)];
}

/// <summary>Mirrors GET /api/services/{id}'s response shape - the only place MediaItems is populated.</summary>
public record ServiceDetailResponse(
    Guid Id,
    Guid CategoryId,
    string Title,
    string Slug,
    string Description,
    string DetailedDescription,
    decimal Price,
    string PriceType,
    decimal? PriceMax,
    bool FeaturedArea,
    int DiscountRate,
    int TaxRate,
    decimal DiscountedPrice,
    bool RequiresPrepayment,
    bool IsEmergencyAvailable,
    Guid? CoverMediaAssetId,
    List<ServiceMediaItemResponse> MediaItems);

public record ServiceMediaItemResponse(Guid Id, Guid MediaAssetId, int DisplayOrder);
