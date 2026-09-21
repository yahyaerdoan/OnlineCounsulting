using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Features.Gallery;

/// <summary>Gallery categories (tags) + gallery items via /api/site-content/gallery-categories and
/// /api/site-content/gallery-items - both lists are public reads, admin CRUD requires auth (handled Api-side).</summary>
public interface IGalleryService
{
    /// <summary>Gets gallery categories (tags).</summary>
    Task<List<GalleryCategoryResponse>> GetCategoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>Creates a gallery category.</summary>
    /// <returns>Envelope carrying the new category id.</returns>
    Task<ApiEnvelope<Guid>> CreateCategoryAsync(string name, string? description = null, CancellationToken cancellationToken = default);

    /// <summary>Updates a gallery category.</summary>
    Task<ApiEnvelope> UpdateCategoryAsync(Guid id, string name, string? description = null, CancellationToken cancellationToken = default);

    /// <summary>Deletes a gallery category.</summary>
    Task<ApiEnvelope> DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Gets gallery items.</summary>
    Task<List<GalleryItemResponse>> GetItemsAsync(CancellationToken cancellationToken = default);

    /// <summary>Creates a gallery item.</summary>
    /// <returns>Envelope carrying the new item id.</returns>
    Task<ApiEnvelope<Guid>> CreateItemAsync(string description, List<Guid> categoryIds, Guid? photoMediaAssetId = null, int displayOrder = 0, Dictionary<string, object>? metadata = null, CancellationToken cancellationToken = default);

    /// <summary>Updates a gallery item.</summary>
    Task<ApiEnvelope> UpdateItemAsync(Guid id, string description, List<Guid> categoryIds, Guid? photoMediaAssetId = null, int displayOrder = 0, Dictionary<string, object>? metadata = null, CancellationToken cancellationToken = default);

    /// <summary>Deletes a gallery item.</summary>
    Task<ApiEnvelope> DeleteItemAsync(Guid id, CancellationToken cancellationToken = default);
}

public record GalleryCategoryResponse(Guid Id, string Name, string? Description);

public record GalleryItemResponse(Guid Id, string Description, Guid? PhotoMediaAssetId, int DisplayOrder, Dictionary<string, object>? Metadata, List<GalleryCategoryResponse> Categories);
