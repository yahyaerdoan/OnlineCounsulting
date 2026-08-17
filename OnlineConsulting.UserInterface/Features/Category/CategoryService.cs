using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Features.Category;

public class CategoryService(IApiClient apiClient) : ICategoryService
{
    private const string CategoriesPath = "/api/categories";

    public async Task<List<CategoryResponse>> GetAllAsync(int? index = null, int? size = null, CancellationToken cancellationToken = default)
    {
        var query = BuildPageQuery(index, size);
        var result = await apiClient.GetAsync<Paginated<CategoryResponse>>($"{CategoriesPath}{query}", cancellationToken);
        return result.ResultData?.Items ?? [];
    }

    public async Task<CategoryResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<CategoryResponse>($"{CategoriesPath}/{id}", cancellationToken);
        return result.ResultData;
    }

    public Task<ApiEnvelope<Guid>> CreateAsync(string title, string description, string icon, string? iconColor, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync<Guid>(CategoriesPath, new { Title = title, Description = description, Icon = icon, IconColor = iconColor }, cancellationToken);

    public Task<ApiEnvelope> UpdateAsync(Guid id, string title, string description, string icon, string? iconColor, CancellationToken cancellationToken = default) =>
        apiClient.PutAsync($"{CategoriesPath}/{id}", new { Title = title, Description = description, Icon = icon, IconColor = iconColor }, cancellationToken);

    public Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiClient.DeleteAsync($"{CategoriesPath}/{id}", cancellationToken);

    private static string BuildPageQuery(int? index, int? size) =>
        index is null && size is null ? string.Empty : $"?index={index}&size={size}";

    private record Paginated<T>(List<T> Items, int Index, int Size, int Count, int Pages);
}
