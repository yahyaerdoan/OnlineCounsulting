namespace OnlineConsulting.UserInterface.Features.Home;

/// <summary>CoverImageUrl/CategoryTitle are resolved server-side (MediaAssetId/CategoryId are plain cross-module
/// ids in ServiceCatalogResponse, not navigation properties) so the view can render them directly.</summary>
public record HomeFeaturedServiceViewModel(Guid Id, string Title, string Slug, string Description, string CategoryTitle, decimal Price, decimal DiscountedPrice, int DiscountRate, string? CoverImageUrl);
