using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.SystemRole;

/// <summary>All Api orchestration for the SystemRole admin screens. This controller previously called ISender
/// directly (bypassing IApiClient/HTTP) since it was already MediatR-based - moved to the same
/// "controllers never call ISender/IApiClient directly" rule as every other migrated slice.</summary>
public interface ISystemRoleService
{
    Task<List<SystemRoleListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UpdateSystemRoleViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> CreateAsync(CreateSystemRoleViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> UpdateAsync(UpdateSystemRoleViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AssignRolePermissionsViewModel?> GetPermissionsAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> AssignPermissionsAsync(Guid roleId, List<string> permissions, CancellationToken cancellationToken = default);
}
