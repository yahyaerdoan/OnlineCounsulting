using OnlineConsulting.Maui.Shared.Infrastructure.Api;
using System.Security.Claims;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Auth;

/// <summary>Builds the host-agnostic base claim set; returns a list, not a finished ClaimsIdentity, since each host appends its own claims on top.</summary>
public static class UserClaimsFactory
{
    public static List<Claim> BuildBaseClaims(CurrentUserResponse user) =>
    [
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.GivenName, user.FirstName),
        new Claim(ClaimTypes.Surname, user.LastName),
        new Claim(ClaimTypes.Email, user.Email),
        .. user.Roles.Select(role => new Claim(ClaimTypes.Role, role)),
        .. user.IsSuperAdmin ? [new Claim(AuthClaimTypes.IsSuperAdmin, "true")] : Array.Empty<Claim>(),
    ];
}
