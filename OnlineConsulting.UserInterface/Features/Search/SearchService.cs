using OnlineConsulting.UserInterface.Infrastructure.Api;
using OnlineConsulting.UserInterface.Infrastructure.Media;

namespace OnlineConsulting.UserInterface.Features.Search;

public class SearchService(IApiClient apiClient, IMediaService mediaService) : ISearchService
{
    public async Task<List<SearchResultItemViewModel>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
            return [];

        var result = await apiClient.GetAsync<List<ServiceResponse>>($"/api/services/search?query={Uri.EscapeDataString(query)}", cancellationToken);
        var services = result.ResultData ?? [];

        var items = new List<SearchResultItemViewModel>();
        foreach (var service in services)
        {
            var coverImageUrl = await mediaService.ResolveUrlAsync(service.CoverMediaAssetId, cancellationToken);
            items.Add(new SearchResultItemViewModel(service.Id, service.Title, service.Slug, service.Description, coverImageUrl));
        }

        return items;
    }

    private record ServiceResponse(Guid Id, string Title, string Slug, string Description, Guid? CoverMediaAssetId);
}
