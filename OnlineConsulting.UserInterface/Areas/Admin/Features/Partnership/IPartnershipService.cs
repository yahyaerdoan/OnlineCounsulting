using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Partnership;

/// <summary>All Api orchestration for the Partnership admin screens - PartnershipController only calls this and
/// renders the result, it never talks to IApiClient/IMediaService directly (single responsibility: the controller
/// owns HTTP/view concerns, this owns "how a partnership view model maps to and from the Api").</summary>
public interface IPartnershipService
{
    Task<List<PartnershipListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Same partnerships as GetAllAsync, but with each partner's social links attached - used by the
    /// public Home page showcase partial, which renders per-partner social icons (the admin CRUD screens don't
    /// need this since social links are managed on their own separate screen).</summary>
    Task<List<PartnershipShowcaseItemViewModel>> GetAllWithSocialLinksAsync(CancellationToken cancellationToken = default);

    Task<UpdatePartnershipViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> CreateAsync(CreatePartnershipViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> UpdateAsync(UpdatePartnershipViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
