using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.PartnershipSocialLink;

/// <summary>All Api orchestration for the nested "manage a partnership's social links" admin screens
/// (reached from the Partnership edit screen with a partnershipId) - backed by SiteContent's
/// PartnershipSocialLink entity via /api/site-content/partnership-social-links.</summary>
public interface IPartnershipSocialLinkService
{
    Task<List<PartnershipSocialLinkListItemViewModel>> GetAllByPartnershipAsync(Guid partnershipId, CancellationToken cancellationToken = default);
    Task<UpdatePartnershipSocialLinkViewModel?> GetByIdAsync(Guid partnershipId, Guid id, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> CreateAsync(CreatePartnershipSocialLinkViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> UpdateAsync(UpdatePartnershipSocialLinkViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
