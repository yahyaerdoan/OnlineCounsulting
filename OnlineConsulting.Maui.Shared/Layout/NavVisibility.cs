using OnlineConsulting.Maui.Shared.Infrastructure.Auth;
using System.Security.Claims;

namespace OnlineConsulting.Maui.Shared.Layout;

/// <summary>One rule for which admin screens a user may use, shared by the sidebar, the command palette and the page guard.</summary>
public static class NavVisibility
{
    public static IReadOnlyList<NavItem> VisibleItems(this NavSection section, ClaimsPrincipal user) =>
        [.. section.Items.Where(item => item.IsVisibleTo(user))];

    public static bool IsVisibleTo(this NavItem item, ClaimsPrincipal user)
    {
        if (user.IsSuperAdmin())
        {
            return true;
        }

        if (item.SuperAdminOnly)
        {
            return false;
        }

        if (user.IsTenantAdmin())
        {
            return true;
        }

        return !item.AdminOnly && (item.RequiredPermission is null || user.HasPermissionArea(item.RequiredPermission));
    }

    /// <summary>False when the path belongs to a nav item the user can't see; its sub-pages (new, edit, detail) follow it.</summary>
    public static bool CanOpen(this IEnumerable<NavSection> sections, string path, ClaimsPrincipal user)
    {
        var owner = sections
            .SelectMany(section => section.Items)
            .Where(item => item.Href != AppRoutes.AdminHome && IsUnder(path, item.Href))
            .MaxBy(item => item.Href.Length);

        return owner is null || owner.IsVisibleTo(user);
    }

    private static bool IsUnder(string path, string href) =>
        path.Equals(href, StringComparison.OrdinalIgnoreCase) || path.StartsWith(href + "/", StringComparison.OrdinalIgnoreCase);
}
