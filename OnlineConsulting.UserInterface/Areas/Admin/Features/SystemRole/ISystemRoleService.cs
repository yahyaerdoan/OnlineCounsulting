using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.SystemRole;

/// <summary>SystemRole admin orchestration - moved off the old direct-ISender bypass to match other migrated slices.</summary>
public interface ISystemRoleService
{
    /// <summary>Fetches all system roles.</summary>
    Task<List<SystemRoleListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Fetches a single role for editing, or null if not found.</summary>
    Task<UpdateSystemRoleViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new system role.</summary>
    Task<ApiEnvelope> CreateAsync(CreateSystemRoleViewModel model, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing system role.</summary>
    Task<ApiEnvelope> UpdateAsync(UpdateSystemRoleViewModel model, CancellationToken cancellationToken = default);

    /// <summary>Deletes a system role by id.</summary>
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Builds the permission-assignment form: the full permission catalog with this role's grants checked.</summary>
    Task<AssignRolePermissionsViewModel?> GetPermissionsAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>Replaces the role's granted permissions with the given set.</summary>
    Task<ApiEnvelope> AssignPermissionsAsync(Guid roleId, List<string> permissions, CancellationToken cancellationToken = default);
}
