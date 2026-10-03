using System.Security.Claims;
using OnlineConsulting.Maui.Shared.Layout;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Auth;

/// <summary>Where a signed-in user lands: storefront customers go to their dashboard, staff to the admin home; a ReturnUrl is honored only when it is a local path.</summary>
public static class SignInRedirect
{
    public const string CustomerHome = "/user/dashboard";

    public static string Target(IReadOnlyCollection<string> roles, string? returnUrl) =>
        IsLocal(returnUrl) ? returnUrl : HomeFor(roles);

    public static string Target(ClaimsPrincipal user, string? returnUrl) =>
        Target([.. user.FindAll(ClaimTypes.Role).Select(c => c.Value)], returnUrl);

    private static string HomeFor(IReadOnlyCollection<string> roles) =>
        roles.Count == 1 && roles.First().Equals(AppRoles.User, StringComparison.OrdinalIgnoreCase) ? CustomerHome : AppRoutes.AdminHome;

    private static bool IsLocal([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] string? url) =>
        url is { Length: > 0 } && url[0] == '/' && (url.Length == 1 || url[1] is not '/' and not '\\');
}
