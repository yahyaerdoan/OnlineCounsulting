using OnlineConsulting.UserInterface.Infrastructure.Api;
using OnlineConsulting.UserInterface.Infrastructure.Media;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.FooterAbout;

public class FooterAboutService(IApiClient apiClient, IMediaService mediaService) : IFooterAboutService
{
    private const string FooterInfosPath = "/api/site-content/footer-info";

    public async Task<List<FooterAboutListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var infos = (await apiClient.GetAsync<List<FooterInfoResponse>>(FooterInfosPath, cancellationToken)).ResultData ?? [];
        return infos.Select(i => new FooterAboutListItemViewModel(i.Id, i.Description, i.ImageUrl)).ToList();
    }

    public async Task<UpdateFooterAboutViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var info = await FindAsync(id, cancellationToken);
        return info is null
            ? null
            : new UpdateFooterAboutViewModel
            {
                Id = info.Id,
                Description = info.Description,
                ImageUrl = info.ImageUrl,
            };
    }

    public async Task<ApiEnvelope> CreateAsync(CreateFooterAboutViewModel model, CancellationToken cancellationToken = default)
    {
        var imageUrl = await UploadAndResolveAsync(model.Image, cancellationToken) ?? string.Empty;
        return await apiClient.PostAsync(FooterInfosPath, new
        {
            model.Description,
            ImageUrl = imageUrl,
        }, cancellationToken);
    }

    public async Task<ApiEnvelope> UpdateAsync(UpdateFooterAboutViewModel model, CancellationToken cancellationToken = default)
    {
        var imageUrl = await UploadAndResolveAsync(model.Image, cancellationToken) ?? (await FindAsync(model.Id, cancellationToken))?.ImageUrl ?? string.Empty;
        return await apiClient.PutAsync($"{FooterInfosPath}/{model.Id}", new
        {
            model.Description,
            ImageUrl = imageUrl,
        }, cancellationToken);
    }

    public Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiClient.DeleteAsync($"{FooterInfosPath}/{id}", cancellationToken);

    /// <summary>FooterInfo stores a plain ImageUrl string (not a MediaAssetId) - upload then immediately resolve.</summary>
    private async Task<string?> UploadAndResolveAsync(IFormFile? file, CancellationToken cancellationToken)
    {
        var mediaAssetId = await mediaService.UploadAsync(file, cancellationToken);
        return mediaAssetId is null ? null : await mediaService.ResolveUrlAsync(mediaAssetId, cancellationToken);
    }

    private async Task<FooterInfoResponse?> FindAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await apiClient.GetAsync<List<FooterInfoResponse>>(FooterInfosPath, cancellationToken);
        return result.ResultData?.FirstOrDefault(i => i.Id == id);
    }

    private record FooterInfoResponse(Guid Id, string ImageUrl, string Description, int DisplayOrder);
}
