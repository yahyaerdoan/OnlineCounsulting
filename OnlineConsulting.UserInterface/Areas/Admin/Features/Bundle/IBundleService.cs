using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Bundle;

public interface IBundleService
{
    /// <summary>Lists all bundles.</summary>
    Task<List<BundleListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Builds the create form, pre-loaded with the available module offering keys.</summary>
    Task<CreateBundleViewModel> GetCreateFormAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets one bundle for editing, or null if not found.</summary>
    Task<UpdateBundleViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ApiEnvelope> CreateAsync(CreateBundleViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> UpdateAsync(UpdateBundleViewModel model, CancellationToken cancellationToken = default);
}
