using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Tenant;

public interface ITenantService
{
    /// <summary>Lists all tenants.</summary>
    Task<List<TenantListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets tenant detail including available (not yet active) module offerings; null if not found.</summary>
    Task<TenantDetailViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Suspends a tenant.</summary>
    Task<ApiEnvelope> SuspendAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Reactivates a suspended tenant.</summary>
    Task<ApiEnvelope> ReactivateAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Adds a module subscription to a tenant.</summary>
    Task<ApiEnvelope> AddModuleAsync(Guid id, string moduleKey, CancellationToken cancellationToken = default);

    /// <summary>Removes a module subscription from a tenant.</summary>
    Task<ApiEnvelope> RemoveModuleAsync(Guid id, string moduleKey, CancellationToken cancellationToken = default);
}
