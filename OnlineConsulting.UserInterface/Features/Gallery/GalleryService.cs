using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Features.Gallery;

public class GalleryService(IApiClient apiClient) : IGalleryService
{
    private const string GalleryCategoriesPath = "/api/site-content/gallery-categories";
    private const string GalleryItemsPath = "/api/site-content/gallery-items";

    public async Task<List<GalleryCategoryResponse>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<List<GalleryCategoryResponse>>(GalleryCategoriesPath, cancellationToken);
        return result.ResultData ?? [];
    }

    public Task<ApiEnvelope<Guid>> CreateCategoryAsync(string name, string? description = null, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync<Guid>(GalleryCategoriesPath, new { Name = name, Description = description }, cancellationToken);

    public Task<ApiEnvelope> UpdateCategoryAsync(Guid id, string name, string? description = null, CancellationToken cancellationToken = default) =>
        apiClient.PutAsync($"{GalleryCategoriesPath}/{id}", new { Name = name, Description = description }, cancellationToken);

    public Task<ApiEnvelope> DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiClient.DeleteAsync($"{GalleryCategoriesPath}/{id}", cancellationToken);

    public async Task<List<GalleryItemResponse>> GetItemsAsync(CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<List<GalleryItemResponse>>(GalleryItemsPath, cancellationToken);
        return result.ResultData ?? [];
    }

    public Task<ApiEnvelope<Guid>> CreateItemAsync(string description, List<Guid> categoryIds, Guid? photoMediaAssetId = null, int displayOrder = 0, Dictionary<string, object>? metadata = null, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync<Guid>(GalleryItemsPath, new { Description = description, CategoryIds = categoryIds, PhotoMediaAssetId = photoMediaAssetId, DisplayOrder = displayOrder, Metadata = metadata }, cancellationToken);

    public Task<ApiEnvelope> UpdateItemAsync(Guid id, string description, List<Guid> categoryIds, Guid? photoMediaAssetId = null, int displayOrder = 0, Dictionary<string, object>? metadata = null, CancellationToken cancellationToken = default) =>
        apiClient.PutAsync($"{GalleryItemsPath}/{id}", new { Description = description, CategoryIds = categoryIds, PhotoMediaAssetId = photoMediaAssetId, DisplayOrder = displayOrder, Metadata = metadata }, cancellationToken);

    public Task<ApiEnvelope> DeleteItemAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiClient.DeleteAsync($"{GalleryItemsPath}/{id}", cancellationToken);
}
