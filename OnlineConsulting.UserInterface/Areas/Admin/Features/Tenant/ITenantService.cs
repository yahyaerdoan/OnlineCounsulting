using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Tenant;

public interface ITenantService
{
    Task<List<TenantListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TenantDetailViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> SuspendAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> ReactivateAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> AddModuleAsync(Guid id, string moduleKey, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> RemoveModuleAsync(Guid id, string moduleKey, CancellationToken cancellationToken = default);
}
