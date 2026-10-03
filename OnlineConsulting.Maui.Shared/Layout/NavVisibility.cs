using OnlineConsulting.Maui.Shared.Infrastructure.Auth;
using System.Security.Claims;

namespace OnlineConsulting.Maui.Shared.Layout;

/// <summary>One rule for which nav items a user sees, shared by the sidebar and the command palette.</summary>
public static class NavVisibility
{
    public static IReadOnlyList<NavItem> VisibleItems(this NavSection section, ClaimsPrincipal user) =>
        [.. section.Items.Where(item => !item.SuperAdminOnly || user.IsSuperAdmin())];
}
