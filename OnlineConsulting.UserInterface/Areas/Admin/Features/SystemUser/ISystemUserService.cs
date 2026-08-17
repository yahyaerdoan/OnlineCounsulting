using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.SystemUser;

/// <summary>All Api orchestration for the SystemUser admin screens. This controller previously called ISender
/// directly (bypassing IApiClient/HTTP) since it was already MediatR-based - moved to the same
/// "controllers never call ISender/IApiClient directly" rule as every other migrated slice.</summary>
public interface ISystemUserService
{
    Task<List<SystemUserListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<RoleAssignmentViewModel>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> AssignRolesAsync(Guid userId, List<RoleAssignmentViewModel> assignments, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
