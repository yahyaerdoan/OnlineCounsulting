using OnlineConsulting.UserInterface.Infrastructure.Api;
using OnlineConsulting.UserInterface.Infrastructure.Media;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Breadcrumb;

public class BreadcrumbService(IApiClient apiClient, IMediaService mediaService) : IBreadcrumbService
{
    private const string PageBannersPath = "/api/site-content/page-banners";

    public async Task<List<BreadcrumbListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var banners = (await apiClient.GetAsync<List<PageBannerResponse>>(PageBannersPath, cancellationToken)).ResultData ?? [];
        return banners.Select(b => new BreadcrumbListItemViewModel(b.Id, b.Title, b.Description, b.ImageUrl)).ToList();
    }

    public async Task<UpdateBreadcrumbViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var banner = await FindAsync(id, cancellationToken);
        return banner is null
            ? null
            : new UpdateBreadcrumbViewModel
            {
                Id = banner.Id,
                Title = banner.Title,
                Description = banner.Description,
                ImageUrl = banner.ImageUrl,
            };
    }

    public async Task<ApiEnvelope> CreateAsync(CreateBreadcrumbViewModel model, CancellationToken cancellationToken = default)
    {
        var imageUrl = await UploadAndResolveAsync(model.Image, cancellationToken) ?? string.Empty;
        return await apiClient.PostAsync(PageBannersPath, new
        {
            model.Title,
            model.Description,
            ImageUrl = imageUrl,
        }, cancellationToken);
    }

    public async Task<ApiEnvelope> UpdateAsync(UpdateBreadcrumbViewModel model, CancellationToken cancellationToken = default)
    {
        var imageUrl = await UploadAndResolveAsync(model.Image, cancellationToken) ?? (await FindAsync(model.Id, cancellationToken))?.ImageUrl ?? string.Empty;
        return await apiClient.PutAsync($"{PageBannersPath}/{model.Id}", new
        {
            model.Title,
            model.Description,
            ImageUrl = imageUrl,
        }, cancellationToken);
    }

    public Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiClient.DeleteAsync($"{PageBannersPath}/{id}", cancellationToken);

    /// <summary>PageBanner stores a plain ImageUrl string (not a MediaAssetId) - upload then immediately resolve.</summary>
    private async Task<string?> UploadAndResolveAsync(IFormFile? file, CancellationToken cancellationToken)
    {
        var mediaAssetId = await mediaService.UploadAsync(file, cancellationToken);
        return mediaAssetId is null ? null : await mediaService.ResolveUrlAsync(mediaAssetId, cancellationToken);
    }

    private async Task<PageBannerResponse?> FindAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await apiClient.GetAsync<List<PageBannerResponse>>(PageBannersPath, cancellationToken);
        return result.ResultData?.FirstOrDefault(b => b.Id == id);
    }

    private record PageBannerResponse(Guid Id, string Title, string Description, string ImageUrl, int DisplayOrder);
}
