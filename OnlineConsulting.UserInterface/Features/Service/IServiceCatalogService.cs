using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Features.Service;

/// <summary>Wraps the Services module's Api endpoints - cover photo is CoverMediaAssetId (resolve via
/// IMediaService), the extended gallery is MediaItems (AddMediaItemAsync/RemoveMediaItemAsync replace what used
/// to be a separate ServiceImageController).</summary>
public interface IServiceCatalogService
{
    Task<List<ServiceCatalogResponse>> GetAllAsync(int? index = null, int? size = null, CancellationToken cancellationToken = default);
    Task<List<ServiceCatalogResponse>> GetByCategoryAsync(Guid categoryId, int? index = null, int? size = null, CancellationToken cancellationToken = default);
    Task<ServiceCatalogResponse?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<ServiceCatalogResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ServiceCatalogResponse>> SearchAsync(string query, CancellationToken cancellationToken = default);
    Task<List<ServiceCatalogResponse>> GetFeaturedAsync(CancellationToken cancellationToken = default);

    Task<ApiEnvelope<Guid>> CreateAsync(Guid categoryId, string title, string description, string detailedDescription, decimal price, bool featuredArea, int discountRate, int taxRate, bool requiresPrepayment = false, Guid? coverMediaAssetId = null, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> UpdateAsync(Guid id, Guid categoryId, string title, string description, string detailedDescription, decimal price, bool featuredArea, int discountRate, int taxRate, bool requiresPrepayment, Guid? coverMediaAssetId = null, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ApiEnvelope<Guid>> AddMediaItemAsync(Guid serviceId, Guid mediaAssetId, int displayOrder = 0, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> RemoveMediaItemAsync(Guid id, CancellationToken cancellationToken = default);
}

public record ServiceMediaItemResponse(Guid Id, Guid MediaAssetId, int DisplayOrder);

public record ServiceCatalogResponse(
    Guid Id,
    Guid CategoryId,
    string Title,
    string Slug,
    string Description,
    string DetailedDescription,
    decimal Price,
    bool FeaturedArea,
    int DiscountRate,
    int TaxRate,
    decimal DiscountedPrice,
    bool RequiresPrepayment,
    Guid? CoverMediaAssetId,
    List<ServiceMediaItemResponse> MediaItems);
