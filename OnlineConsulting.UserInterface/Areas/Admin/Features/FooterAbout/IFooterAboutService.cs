using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.FooterAbout;

/// <summary>All Api orchestration for the FooterAbout admin screens (Api concept is called FooterInfo - the C#
/// type/route name stays FooterAbout for continuity with the existing view folder).</summary>
public interface IFooterAboutService
{
    Task<List<FooterAboutListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UpdateFooterAboutViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> CreateAsync(CreateFooterAboutViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> UpdateAsync(UpdateFooterAboutViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
