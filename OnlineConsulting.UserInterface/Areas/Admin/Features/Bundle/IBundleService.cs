using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Bundle;

public interface IBundleService
{
    Task<List<BundleListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CreateBundleViewModel> GetCreateFormAsync(CancellationToken cancellationToken = default);
    Task<UpdateBundleViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> CreateAsync(CreateBundleViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> UpdateAsync(UpdateBundleViewModel model, CancellationToken cancellationToken = default);
}
