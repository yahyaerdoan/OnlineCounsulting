using System.Security.Claims;
using OnlineConsulting.Maui.Shared.Infrastructure.Api;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Auth;

/// <summary>Builds the base claim set from a CurrentUserResponse - identical on every host. Each
/// host still appends its own host-specific claims on top (e.g. the Web host's cookie-carried
/// access token), so this returns a list rather than a finished ClaimsIdentity.</summary>
public static class UserClaimsFactory
{
    public static List<Claim> BuildBaseClaims(CurrentUserResponse user) =>
    [
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.GivenName, user.FirstName),
        new Claim(ClaimTypes.Email, user.Email),
        .. user.Roles.Select(role => new Claim(ClaimTypes.Role, role)),
        .. user.IsSuperAdmin ? [new Claim(AuthClaimTypes.IsSuperAdmin, "true")] : Array.Empty<Claim>(),
    ];
}
