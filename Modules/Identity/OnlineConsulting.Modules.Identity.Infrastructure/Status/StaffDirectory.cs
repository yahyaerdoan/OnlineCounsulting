using Core.PersistenceLayer.MultiTenancy;
using Core.SecurityLayer.Constants;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Identity.Application.Features.Auth;
using OnlineConsulting.Modules.Identity.Infrastructure.Persistence;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Identity;

namespace OnlineConsulting.Modules.Identity.Infrastructure.Status;

/// <summary>Resolves permissions the same way sign-in does (RolePermissionResolver): role claims, FullAccess as a bypass that can't be
/// denied, then the user's individual deny overrides.</summary>
public class StaffDirectory(AppIdentityDbContext context) : IStaffDirectory
{
    public async Task<IReadOnlyList<Guid>> GetUserIdsWithAnyPermissionAsync(Guid tenantId, IReadOnlyCollection<string> permissions, CancellationToken cancellationToken = default)
    {
        List<string> granting = [.. permissions, PermissionClaimTypes.FullAccess];

        var grants = await (
            from user in context.Users.IgnoreTenantFilter()
            where user.TenantId == tenantId && user.IsActive && user.DeletedDate == null
            join userRole in context.UserRoles on user.Id equals userRole.UserId
            join roleClaim in context.RoleClaims on userRole.RoleId equals roleClaim.RoleId
            where roleClaim.ClaimType == PermissionClaimTypes.Type && granting.Contains(roleClaim.ClaimValue ?? "")
            select new { user.Id, Permission = roleClaim.ClaimValue ?? "" })
            .Distinct()
            .ToListAsync(cancellationToken);

        if (grants.Count == 0)
        {
            return [];
        }

        var candidateIds = grants.Select(g => g.Id).Distinct().ToList();
        var denies = (await context.UserClaims
                .Where(c => candidateIds.Contains(c.UserId) && c.ClaimType == PermissionOverrideClaimTypes.Deny)
                .Select(c => new { c.UserId, Permission = c.ClaimValue ?? "" })
                .ToListAsync(cancellationToken))
            .Select(d => (d.UserId, d.Permission))
            .ToHashSet();

        return [.. grants
            .Where(g => g.Permission == PermissionClaimTypes.FullAccess || !denies.Contains((g.Id, g.Permission)))
            .Select(g => g.Id)
            .Distinct()];
    }
}
