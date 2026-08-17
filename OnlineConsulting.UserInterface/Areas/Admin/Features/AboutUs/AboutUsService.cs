using OnlineConsulting.UserInterface.Infrastructure.Api;
using OnlineConsulting.UserInterface.Infrastructure.Media;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.AboutUs;

public class AboutUsService(IApiClient apiClient, IMediaService mediaService) : IAboutUsService
{
    private const string AboutUsPath = "/api/site-content/about-us";

    public async Task<List<AboutUsListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = (await apiClient.GetAsync<List<AboutUsResponse>>(AboutUsPath, cancellationToken)).ResultData ?? [];
        return items.Select(a => new AboutUsListItemViewModel(a.Id, a.Title, a.Description, a.CoverImage, a.VideoUrl)).ToList();
    }

    public async Task<UpdateAboutUsViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var aboutUs = await FindAsync(id, cancellationToken);
        if (aboutUs is null)
            return null;

        return new UpdateAboutUsViewModel
        {
            Id = aboutUs.Id,
            Title = aboutUs.Title,
            Description = aboutUs.Description,
            VideoUrl = aboutUs.VideoUrl,
            CoverImage = aboutUs.CoverImage,
        };
    }

    public async Task<ApiEnvelope> CreateAsync(CreateAboutUsViewModel model, CancellationToken cancellationToken = default)
    {
        var coverImage = await UploadAndResolveAsync(model.Image, cancellationToken);
        return await apiClient.PostAsync(AboutUsPath, new
        {
            model.Title,
            model.Description,
            CoverImage = coverImage,
            model.VideoUrl,
        }, cancellationToken);
    }

    public async Task<ApiEnvelope> UpdateAsync(UpdateAboutUsViewModel model, CancellationToken cancellationToken = default)
    {
        var coverImage = await UploadAndResolveAsync(model.Image, cancellationToken) ?? (await FindAsync(model.Id, cancellationToken))?.CoverImage;
        return await apiClient.PutAsync($"{AboutUsPath}/{model.Id}", new
        {
            model.Title,
            model.Description,
            CoverImage = coverImage,
            model.VideoUrl,
        }, cancellationToken);
    }

    public Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiClient.DeleteAsync($"{AboutUsPath}/{id}", cancellationToken);

    /// <summary>AboutUs stores a plain CoverImage string (not a MediaAssetId) - upload then immediately resolve.</summary>
    private async Task<string?> UploadAndResolveAsync(IFormFile? file, CancellationToken cancellationToken)
    {
        var mediaAssetId = await mediaService.UploadAsync(file, cancellationToken);
        return mediaAssetId is null ? null : await mediaService.ResolveUrlAsync(mediaAssetId, cancellationToken);
    }

    private async Task<AboutUsResponse?> FindAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await apiClient.GetAsync<List<AboutUsResponse>>(AboutUsPath, cancellationToken);
        return result.ResultData?.FirstOrDefault(a => a.Id == id);
    }

    private record AboutUsResponse(Guid Id, string Title, string Description, string? CoverImage, string? VideoUrl, int DisplayOrder);
}
