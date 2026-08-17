using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Features.Gallery;

/// <summary>Gallery categories (tags) + gallery items via /api/site-content/gallery-categories and
/// /api/site-content/gallery-items - both lists are public reads, admin CRUD requires auth (handled Api-side).</summary>
public interface IGalleryService
{
    Task<List<GalleryCategoryResponse>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<ApiEnvelope<Guid>> CreateCategoryAsync(string name, string? description = null, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> UpdateCategoryAsync(Guid id, string name, string? description = null, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default);

    Task<List<GalleryItemResponse>> GetItemsAsync(CancellationToken cancellationToken = default);
    Task<ApiEnvelope<Guid>> CreateItemAsync(string description, List<Guid> categoryIds, Guid? photoMediaAssetId = null, int displayOrder = 0, Dictionary<string, object>? metadata = null, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> UpdateItemAsync(Guid id, string description, List<Guid> categoryIds, Guid? photoMediaAssetId = null, int displayOrder = 0, Dictionary<string, object>? metadata = null, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> DeleteItemAsync(Guid id, CancellationToken cancellationToken = default);
}

public record GalleryCategoryResponse(Guid Id, string Name, string? Description);

public record GalleryItemResponse(Guid Id, string Description, Guid? PhotoMediaAssetId, int DisplayOrder, Dictionary<string, object>? Metadata, List<GalleryCategoryResponse> Categories);
