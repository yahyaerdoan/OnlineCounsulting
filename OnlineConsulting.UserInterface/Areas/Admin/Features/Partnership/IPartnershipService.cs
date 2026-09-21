using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Partnership;

/// <summary>All Api orchestration for the Partnership admin screens - PartnershipController never talks to IApiClient/IMediaService directly.</summary>
public interface IPartnershipService
{
    /// <summary>Fetches all partnerships.</summary>
    Task<List<PartnershipListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Same as GetAllAsync but with social links attached, for the public Home page showcase partial.</summary>
    Task<List<PartnershipShowcaseItemViewModel>> GetAllWithSocialLinksAsync(CancellationToken cancellationToken = default);

    /// <summary>Fetches a single partnership for editing, or null if not found.</summary>
    Task<UpdatePartnershipViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new partnership, uploading its photo if provided.</summary>
    Task<ApiEnvelope> CreateAsync(CreatePartnershipViewModel model, CancellationToken cancellationToken = default);

    /// <summary>Updates a partnership, keeping the existing photo when none is uploaded.</summary>
    Task<ApiEnvelope> UpdateAsync(UpdatePartnershipViewModel model, CancellationToken cancellationToken = default);

    /// <summary>Deletes a partnership by id.</summary>
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
