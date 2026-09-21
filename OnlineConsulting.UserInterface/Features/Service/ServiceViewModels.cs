namespace OnlineConsulting.UserInterface.Features.Service;

/// <summary>CoverImageUrl/CategoryTitle are resolved server-side (ServiceCatalogResponse only carries plain
/// CategoryId/CoverMediaAssetId, no navigation).</summary>
public record ServiceCardViewModel(Guid Id, string Title, string Slug, string Description, string CategoryTitle, decimal Price, decimal DiscountedPrice, int DiscountRate, string? CoverImageUrl);

/// <summary>TotalCount is computed by fetching every service and paging client-side, since GetAllAsync returns no total count - acceptable at this catalog's size.</summary>
public record ServiceListViewModel(List<ServiceCardViewModel> Services, int Page, int Size, int TotalCount)
{
    public int TotalPages => Size <= 0 ? 0 : (int)Math.Ceiling((double)TotalCount / Size);
}

/// <summary>ImageUrls is the cover image (if any) followed by every MediaItem, in display order - the old
/// ServiceImages carousel becomes this flat resolved list.</summary>
public record ServiceDetailViewModel(Guid Id, string Title, string Slug, string Description, string DetailedDescription, Guid CategoryId, string CategoryTitle, decimal Price, decimal DiscountedPrice, int DiscountRate, List<string> ImageUrls);
