using System.Security.Claims;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Auth;

public static class ClaimsPrincipalExtensions
{
    public static bool IsSuperAdmin(this ClaimsPrincipal user) =>
        user.HasClaim(c => c.Type == AuthClaimTypes.IsSuperAdmin);

    /// <summary>Business owner; holds every permission in its own tenant.</summary>
    public static bool IsTenantAdmin(this ClaimsPrincipal user) => user.IsInRole(AppRoles.Admin);

    /// <summary>Can open the admin panel: any role besides User, the same rule SignInRedirect uses.</summary>
    public static bool IsStaff(this ClaimsPrincipal user) =>
        user.FindAll(ClaimTypes.Role).Any(role => !role.Value.Equals(AppRoles.User, StringComparison.OrdinalIgnoreCase));

    public static bool HasPermissionArea(this ClaimsPrincipal user, string area) =>
        user.HasClaim(AuthClaimTypes.PermissionArea, area);
}
