using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Breadcrumb;

/// <summary>All Api orchestration for the Breadcrumb admin screens (Api concept is called PageBanner - the C#
/// type/route name stays Breadcrumb for continuity with the existing view folder).</summary>
public interface IBreadcrumbService
{
    Task<List<BreadcrumbListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UpdateBreadcrumbViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> CreateAsync(CreateBreadcrumbViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> UpdateAsync(UpdateBreadcrumbViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
