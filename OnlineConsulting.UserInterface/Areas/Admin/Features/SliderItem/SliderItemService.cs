using OnlineConsulting.UserInterface.Infrastructure.Api;
using OnlineConsulting.UserInterface.Infrastructure.Media;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.SliderItem;

public class SliderItemService(IApiClient apiClient, IMediaService mediaService) : ISliderItemService
{
    private const string HeroSlidesPath = "/api/site-content/hero-slides";

    public async Task<List<SliderItemListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var slides = (await apiClient.GetAsync<List<HeroSlideResponse>>(HeroSlidesPath, cancellationToken)).ResultData ?? [];
        return slides.Select(s => new SliderItemListItemViewModel(s.Id, s.Title, s.Description, s.ImageUrl)).ToList();
    }

    public async Task<UpdateSliderItemViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var slide = await FindAsync(id, cancellationToken);
        if (slide is null)
            return null;

        return new UpdateSliderItemViewModel
        {
            Id = slide.Id,
            Title = slide.Title,
            Description = slide.Description,
            ImageUrl = slide.ImageUrl,
        };
    }

    public async Task<ApiEnvelope> CreateAsync(CreateSliderItemViewModel model, CancellationToken cancellationToken = default)
    {
        var imageUrl = await UploadAndResolveAsync(model.Image, cancellationToken) ?? string.Empty;
        return await apiClient.PostAsync(HeroSlidesPath, new
        {
            model.Title,
            model.Description,
            ImageUrl = imageUrl,
        }, cancellationToken);
    }

    public async Task<ApiEnvelope> UpdateAsync(UpdateSliderItemViewModel model, CancellationToken cancellationToken = default)
    {
        var imageUrl = await UploadAndResolveAsync(model.Image, cancellationToken) ?? (await FindAsync(model.Id, cancellationToken))?.ImageUrl ?? string.Empty;
        return await apiClient.PutAsync($"{HeroSlidesPath}/{model.Id}", new
        {
            model.Title,
            model.Description,
            ImageUrl = imageUrl,
        }, cancellationToken);
    }

    public Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiClient.DeleteAsync($"{HeroSlidesPath}/{id}", cancellationToken);

    /// <summary>HeroSlide stores a plain ImageUrl string (not a MediaAssetId) - upload then immediately resolve.</summary>
    private async Task<string?> UploadAndResolveAsync(IFormFile? file, CancellationToken cancellationToken)
    {
        var mediaAssetId = await mediaService.UploadAsync(file, cancellationToken);
        return mediaAssetId is null ? null : await mediaService.ResolveUrlAsync(mediaAssetId, cancellationToken);
    }

    private async Task<HeroSlideResponse?> FindAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await apiClient.GetAsync<List<HeroSlideResponse>>(HeroSlidesPath, cancellationToken);
        return result.ResultData?.FirstOrDefault(s => s.Id == id);
    }

    private record HeroSlideResponse(Guid Id, string Title, string Description, string ImageUrl, int DisplayOrder);
}
