using OnlineConsulting.UserInterface.Infrastructure.Api;
using OnlineConsulting.UserInterface.Infrastructure.Media;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Partnership;

public class PartnershipService(IApiClient apiClient, IMediaService mediaService) : IPartnershipService
{
    private const string PartnershipsPath = "/api/site-content/partnerships";

    public async Task<List<PartnershipListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var partnerships = (await apiClient.GetAsync<List<PartnershipResponse>>(PartnershipsPath, cancellationToken)).ResultData ?? [];

        var items = new List<PartnershipListItemViewModel>();
        foreach (var partnership in partnerships)
        {
            var photoUrl = await mediaService.ResolveUrlAsync(partnership.PhotoMediaAssetId, cancellationToken);
            items.Add(new PartnershipListItemViewModel(partnership.Id, partnership.FirstName, partnership.LastName, partnership.Title, partnership.Description, partnership.Email, partnership.WebsiteUrl, photoUrl));
        }

        return items;
    }

    public async Task<List<PartnershipShowcaseItemViewModel>> GetAllWithSocialLinksAsync(CancellationToken cancellationToken = default)
    {
        var partnerships = (await apiClient.GetAsync<List<PartnershipResponse>>(PartnershipsPath, cancellationToken)).ResultData ?? [];

        var items = new List<PartnershipShowcaseItemViewModel>();
        foreach (var partnership in partnerships)
        {
            var photoUrl = await mediaService.ResolveUrlAsync(partnership.PhotoMediaAssetId, cancellationToken);
            var socialLinks = (partnership.SocialLinks ?? []).Select(s => new PartnershipSocialLinkViewModel(s.Id, s.Name, s.Url, s.Icon, s.IconColor)).ToList();
            items.Add(new PartnershipShowcaseItemViewModel(partnership.Id, partnership.FirstName, partnership.LastName, partnership.Title, partnership.Description,
                partnership.Email, partnership.CompanyName, partnership.WebsiteUrl, photoUrl, socialLinks));
        }

        return items;
    }

    public async Task<UpdatePartnershipViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var partnership = await FindAsync(id, cancellationToken);
        return partnership is null
            ? null
            : new UpdatePartnershipViewModel
            {
                Id = partnership.Id,
                FirstName = partnership.FirstName,
                LastName = partnership.LastName,
                CompanyName = partnership.CompanyName,
                WebsiteUrl = partnership.WebsiteUrl,
                Title = partnership.Title,
                Email = partnership.Email,
                Description = partnership.Description,
                PhotoUrl = await mediaService.ResolveUrlAsync(partnership.PhotoMediaAssetId, cancellationToken),
            };
    }

    public async Task<ApiEnvelope> CreateAsync(CreatePartnershipViewModel model, CancellationToken cancellationToken = default)
    {
        var photoMediaAssetId = await mediaService.UploadAsync(model.Image, cancellationToken);
        return await apiClient.PostAsync(PartnershipsPath, BuildCommand(model, photoMediaAssetId), cancellationToken);
    }

    public async Task<ApiEnvelope> UpdateAsync(UpdatePartnershipViewModel model, CancellationToken cancellationToken = default)
    {
        var photoMediaAssetId = model.Image is { Length: > 0 }
            ? await mediaService.UploadAsync(model.Image, cancellationToken)
            : (await FindAsync(model.Id, cancellationToken))?.PhotoMediaAssetId;

        return await apiClient.PutAsync($"{PartnershipsPath}/{model.Id}", BuildCommand(model, photoMediaAssetId), cancellationToken);
    }

    public Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiClient.DeleteAsync($"{PartnershipsPath}/{id}", cancellationToken);

    private async Task<PartnershipResponse?> FindAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await apiClient.GetAsync<List<PartnershipResponse>>(PartnershipsPath, cancellationToken);
        return result.ResultData?.FirstOrDefault(p => p.Id == id);
    }

    private static object BuildCommand(CreatePartnershipViewModel model, Guid? photoMediaAssetId) => new
    {
        model.FirstName,
        model.LastName,
        model.Email,
        model.Title,
        model.CompanyName,
        model.Description,
        model.WebsiteUrl,
        PhotoMediaAssetId = photoMediaAssetId,
    };

    private record PartnershipResponse(Guid Id, string FirstName, string LastName, string Email, string Title, string CompanyName, string Description, string WebsiteUrl, Guid? PhotoMediaAssetId, int DisplayOrder, List<PartnershipSocialLinkResponse>? SocialLinks = null);

    private record PartnershipSocialLinkResponse(Guid Id, string Name, string Url, string Icon, string? IconColor);
}
