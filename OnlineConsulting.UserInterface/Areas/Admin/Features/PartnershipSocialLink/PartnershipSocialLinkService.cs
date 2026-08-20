using OnlineConsulting.UserInterface.Areas.Admin.Features.Partnership;
using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.PartnershipSocialLink;

public class PartnershipSocialLinkService(IApiClient apiClient, IPartnershipService partnershipService) : IPartnershipSocialLinkService
{
    private const string PartnershipSocialLinksPath = "/api/site-content/partnership-social-links";

    public async Task<List<PartnershipSocialLinkListItemViewModel>> GetAllByPartnershipAsync(Guid partnershipId, CancellationToken cancellationToken = default)
    {
        var partnerships = await partnershipService.GetAllWithSocialLinksAsync(cancellationToken);
        var partnership = partnerships.FirstOrDefault(p => p.Id == partnershipId);
        return partnership is null
            ? []
            : partnership.SocialLinks.Select(s => new PartnershipSocialLinkListItemViewModel(s.Id, partnershipId, s.Name, s.Url, s.Icon, s.IconColor)).ToList();
    }

    public async Task<UpdatePartnershipSocialLinkViewModel?> GetByIdAsync(Guid partnershipId, Guid id, CancellationToken cancellationToken = default)
    {
        var links = await GetAllByPartnershipAsync(partnershipId, cancellationToken);
        var link = links.FirstOrDefault(l => l.Id == id);
        return link is null
            ? null
            : new UpdatePartnershipSocialLinkViewModel
            {
                Id = link.Id,
                PartnershipId = partnershipId,
                Name = link.Name,
                Url = link.Url,
                Icon = link.Icon,
                IconColor = link.IconColor,
            };
    }

    public Task<ApiEnvelope> CreateAsync(CreatePartnershipSocialLinkViewModel model, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync(PartnershipSocialLinksPath, new { model.PartnershipId, model.Name, model.Url, model.Icon, model.IconColor }, cancellationToken);

    public Task<ApiEnvelope> UpdateAsync(UpdatePartnershipSocialLinkViewModel model, CancellationToken cancellationToken = default) =>
        apiClient.PutAsync($"{PartnershipSocialLinksPath}/{model.Id}", new { model.Name, model.Url, model.Icon, model.IconColor }, cancellationToken);

    public Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiClient.DeleteAsync($"{PartnershipSocialLinksPath}/{id}", cancellationToken);
}
