using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Identity.Domain;

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
}
