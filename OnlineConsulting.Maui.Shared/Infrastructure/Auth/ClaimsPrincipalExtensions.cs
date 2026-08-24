using System.Security.Claims;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Auth;

public static class ClaimsPrincipalExtensions
{
    public static bool IsSuperAdmin(this ClaimsPrincipal user) =>
        user.HasClaim(c => c.Type == AuthClaimTypes.IsSuperAdmin);
}
