namespace OnlineConsulting.SharedKernel.Identity;

/// <summary>Cross-module lookup of a tenant's team members who hold a permission, so a module can alert the people who act on its work
/// (e.g. Scheduling telling dispatchers about a new booking) without knowing Identity's role and claim tables.</summary>
public interface IStaffDirectory
{
    /// <summary>Active users of the tenant granted any of the permissions (or full access) through their roles, minus individual denies.</summary>
    Task<IReadOnlyList<Guid>> GetUserIdsWithAnyPermissionAsync(Guid tenantId, IReadOnlyCollection<string> permissions, CancellationToken cancellationToken = default);
}
