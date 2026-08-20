using OnlineConsulting.UserInterface.Infrastructure.Api;
using OnlineConsulting.UserInterface.Infrastructure.Media;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.WhatWeProvide;

public class WhatWeProvideService(IApiClient apiClient, IMediaService mediaService) : IWhatWeProvideService
{
    private const string FeatureHighlightsPath = "/api/site-content/feature-highlights";

    public async Task<List<WhatWeProvideListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var highlights = (await apiClient.GetAsync<List<FeatureHighlightResponse>>(FeatureHighlightsPath, cancellationToken)).ResultData ?? [];
        return highlights.Select(h => new WhatWeProvideListItemViewModel(h.Id, h.Title, h.Description, h.ImageUrl)).ToList();
    }

    public async Task<UpdateWhatWeProvideViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var highlight = await FindAsync(id, cancellationToken);
        return highlight is null
            ? null
            : new UpdateWhatWeProvideViewModel
            {
                Id = highlight.Id,
                Title = highlight.Title,
                Description = highlight.Description,
                ImageUrl = highlight.ImageUrl,
            };
    }

    public async Task<ApiEnvelope> CreateAsync(CreateWhatWeProvideViewModel model, CancellationToken cancellationToken = default)
    {
        var imageUrl = await UploadAndResolveAsync(model.Image, cancellationToken) ?? string.Empty;
        return await apiClient.PostAsync(FeatureHighlightsPath, new
        {
            model.Title,
            model.Description,
            ImageUrl = imageUrl,
        }, cancellationToken);
    }

    public async Task<ApiEnvelope> UpdateAsync(UpdateWhatWeProvideViewModel model, CancellationToken cancellationToken = default)
    {
        var imageUrl = await UploadAndResolveAsync(model.Image, cancellationToken) ?? (await FindAsync(model.Id, cancellationToken))?.ImageUrl ?? string.Empty;
        return await apiClient.PutAsync($"{FeatureHighlightsPath}/{model.Id}", new
        {
            model.Title,
            model.Description,
            ImageUrl = imageUrl,
        }, cancellationToken);
    }

    public Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiClient.DeleteAsync($"{FeatureHighlightsPath}/{id}", cancellationToken);

    /// <summary>FeatureHighlight stores a plain ImageUrl string (not a MediaAssetId) - upload then immediately resolve.</summary>
    private async Task<string?> UploadAndResolveAsync(IFormFile? file, CancellationToken cancellationToken)
    {
        var mediaAssetId = await mediaService.UploadAsync(file, cancellationToken);
        return mediaAssetId is null ? null : await mediaService.ResolveUrlAsync(mediaAssetId, cancellationToken);
    }

    private async Task<FeatureHighlightResponse?> FindAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await apiClient.GetAsync<List<FeatureHighlightResponse>>(FeatureHighlightsPath, cancellationToken);
        return result.ResultData?.FirstOrDefault(h => h.Id == id);
    }

    private record FeatureHighlightResponse(Guid Id, string Title, string Description, string ImageUrl, int DisplayOrder);
}
