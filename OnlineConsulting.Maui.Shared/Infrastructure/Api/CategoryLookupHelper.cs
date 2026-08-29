namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Fetches every category for a dropdown or id-to-title lookup - shared by any page that
/// needs the full category list rather than a paginated slice.</summary>
public static class CategoryLookupHelper
{
    public static async Task<List<CategoryResponse>> GetAllAsync(IApiClient apiClient, CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<PaginatedResponse<CategoryResponse>>($"{ApiRoutes.Categories.Base}?size=100", cancellationToken);
        return result.IsSuccessful && result.ResultData is not null ? result.ResultData.Items : [];
    }
}
