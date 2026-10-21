using MudBlazor;

namespace OnlineConsulting.Maui.Shared.Layout;

/// <summary>Declarative nav data so the drawer markup stays one loop instead of one hand-written
/// MudNavLink per screen - add an item here once its page exists under Pages/{Module}.</summary>
/// <param name="RequiredPermission">Permission area (see PermissionAreas) a Member needs; business owners and Super Admins see everything.</param>
/// <param name="AdminOnly">Business owner settings, hidden from Members.</param>
public record NavItem(string Text, string Href, string Icon, string? RequiredPermission = null, bool SuperAdminOnly = false, bool AdminOnly = false);

/// <summary>Color is per-section (not per-item) so every item in a group reads as one family at a
/// glance in the sidebar, instead of an arbitrary per-link rainbow.</summary>
public record NavSection(string Title, string Icon, Color Color, IReadOnlyList<NavItem> Items);
