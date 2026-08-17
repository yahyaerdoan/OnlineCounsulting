using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Features.Service;

public class ServiceCatalogService(IApiClient apiClient) : IServiceCatalogService
{
    private const string ServicesPath = "/api/services";

    public async Task<List<ServiceCatalogResponse>> GetAllAsync(int? index = null, int? size = null, CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<Paginated<ServiceCatalogResponse>>($"{ServicesPath}{BuildPageQuery(index, size)}", cancellationToken);
        return result.ResultData?.Items ?? [];
    }

    public async Task<List<ServiceCatalogResponse>> GetByCategoryAsync(Guid categoryId, int? index = null, int? size = null, CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<Paginated<ServiceCatalogResponse>>($"/api/categories/{categoryId}/services{BuildPageQuery(index, size)}", cancellationToken);
        return result.ResultData?.Items ?? [];
    }

    public async Task<ServiceCatalogResponse?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<ServiceCatalogResponse>($"{ServicesPath}/by-slug/{slug}", cancellationToken);
        return result.ResultData;
    }

    public async Task<ServiceCatalogResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<ServiceCatalogResponse>($"{ServicesPath}/{id}", cancellationToken);
        return result.ResultData;
    }

    public async Task<List<ServiceCatalogResponse>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<List<ServiceCatalogResponse>>($"{ServicesPath}/search?query={Uri.EscapeDataString(query)}", cancellationToken);
        return result.ResultData ?? [];
    }

    public async Task<List<ServiceCatalogResponse>> GetFeaturedAsync(CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<List<ServiceCatalogResponse>>($"{ServicesPath}/featured", cancellationToken);
        return result.ResultData ?? [];
    }

    public Task<ApiEnvelope<Guid>> CreateAsync(Guid categoryId, string title, string description, string detailedDescription, decimal price, bool featuredArea, int discountRate, int taxRate, bool requiresPrepayment = false, Guid? coverMediaAssetId = null, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync<Guid>(ServicesPath, new { CategoryId = categoryId, Title = title, Description = description, DetailedDescription = detailedDescription, Price = price, FeaturedArea = featuredArea, DiscountRate = discountRate, TaxRate = taxRate, RequiresPrepayment = requiresPrepayment, CoverMediaAssetId = coverMediaAssetId }, cancellationToken);

    public Task<ApiEnvelope> UpdateAsync(Guid id, Guid categoryId, string title, string description, string detailedDescription, decimal price, bool featuredArea, int discountRate, int taxRate, bool requiresPrepayment, Guid? coverMediaAssetId = null, CancellationToken cancellationToken = default) =>
        apiClient.PutAsync($"{ServicesPath}/{id}", new { CategoryId = categoryId, Title = title, Description = description, DetailedDescription = detailedDescription, Price = price, FeaturedArea = featuredArea, DiscountRate = discountRate, TaxRate = taxRate, RequiresPrepayment = requiresPrepayment, CoverMediaAssetId = coverMediaAssetId }, cancellationToken);

    public Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiClient.DeleteAsync($"{ServicesPath}/{id}", cancellationToken);

    public Task<ApiEnvelope<Guid>> AddMediaItemAsync(Guid serviceId, Guid mediaAssetId, int displayOrder = 0, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync<Guid>($"{ServicesPath}/media-items", new { ServiceId = serviceId, MediaAssetId = mediaAssetId, DisplayOrder = displayOrder }, cancellationToken);

    public Task<ApiEnvelope> RemoveMediaItemAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiClient.DeleteAsync($"{ServicesPath}/media-items/{id}", cancellationToken);

    private static string BuildPageQuery(int? index, int? size) =>
        index is null && size is null ? string.Empty : $"?index={index}&size={size}";

    private record Paginated<T>(List<T> Items, int Index, int Size, int Count, int Pages);
}
