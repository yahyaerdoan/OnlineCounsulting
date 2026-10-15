using Core.PersistenceLayer.MultiTenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.CurrentUser;

namespace OnlineConsulting.Modules.Identity.Application.Common;

/// <summary>Email is unique per tenant, not globally: UserManager.FindByEmailAsync throws once two tenants share an address, so email lookups go through here.</summary>
public static class UserManagerTenantExtensions
{
    /// <summary>The tenant's user with this email, or null.</summary>
    public static Task<User?> FindByEmailInTenantAsync(this UserManager<User> userManager, string email, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = userManager.NormalizeEmail(email);
        return userManager.Users.SingleOrDefaultAsync(u => u.TenantId == tenantId && u.NormalizedEmail == normalizedEmail, cancellationToken);
    }

    /// <summary>The user an admin screen acts on: a Super Admin reaches any tenant's user, everyone else only their own tenant's (the filter hides the rest).
    /// Callers still run their permission checks, and open TenantScope for the user's tenant only after those pass.</summary>
    public static Task<User?> FindManageableUserAsync(this UserManager<User> userManager, Guid userId, ICurrentUserAccessor currentUserAccessor, CancellationToken cancellationToken = default)
    {
        var users = currentUserAccessor.IsInRole(GlobalOperationClaims.SuperAdmin) ? userManager.Users.IgnoreTenantFilter() : userManager.Users;
        return users.SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }
}
