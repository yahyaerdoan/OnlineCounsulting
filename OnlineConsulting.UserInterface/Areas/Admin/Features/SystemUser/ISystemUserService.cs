using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.SystemUser;

/// <summary>SystemUser admin orchestration - moved off the old direct-ISender bypass to match other migrated slices.</summary>
public interface ISystemUserService
{
    /// <summary>Fetches all system users.</summary>
    Task<List<SystemUserListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Fetches the role catalog with this user's current assignments checked.</summary>
    Task<List<RoleAssignmentViewModel>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Replaces the user's role assignments with the given set.</summary>
    Task<ApiEnvelope> AssignRolesAsync(Guid userId, List<RoleAssignmentViewModel> assignments, CancellationToken cancellationToken = default);

    /// <summary>Deletes a system user by id.</summary>
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
