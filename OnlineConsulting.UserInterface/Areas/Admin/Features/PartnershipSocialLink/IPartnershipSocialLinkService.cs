using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.PartnershipSocialLink;

/// <summary>Orchestration for the nested "manage a partnership's social links" admin screens, reached from the Partnership edit screen.</summary>
public interface IPartnershipSocialLinkService
{
    /// <summary>Fetches all social links for a given partnership.</summary>
    Task<List<PartnershipSocialLinkListItemViewModel>> GetAllByPartnershipAsync(Guid partnershipId, CancellationToken cancellationToken = default);

    /// <summary>Fetches a single social link for editing, or null if not found.</summary>
    Task<UpdatePartnershipSocialLinkViewModel?> GetByIdAsync(Guid partnershipId, Guid id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new social link for a partnership.</summary>
    Task<ApiEnvelope> CreateAsync(CreatePartnershipSocialLinkViewModel model, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing social link.</summary>
    Task<ApiEnvelope> UpdateAsync(UpdatePartnershipSocialLinkViewModel model, CancellationToken cancellationToken = default);

    /// <summary>Deletes a social link by id.</summary>
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
