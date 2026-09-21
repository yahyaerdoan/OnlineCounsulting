using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.FooterAbout;

/// <summary>All Api orchestration for the FooterAbout admin screens (Api concept is called FooterInfo - the C#
/// type/route name stays FooterAbout for continuity with the existing view folder).</summary>
public interface IFooterAboutService
{
    /// <summary>Lists all footer-about entries.</summary>
    Task<List<FooterAboutListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets one footer-about entry for editing, or null if not found.</summary>
    Task<UpdateFooterAboutViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new footer-about entry, uploading the image if provided.</summary>
    Task<ApiEnvelope> CreateAsync(CreateFooterAboutViewModel model, CancellationToken cancellationToken = default);

    /// <summary>Updates a footer-about entry, keeping the existing image when none is uploaded.</summary>
    Task<ApiEnvelope> UpdateAsync(UpdateFooterAboutViewModel model, CancellationToken cancellationToken = default);

    /// <summary>Deletes a footer-about entry.</summary>
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
