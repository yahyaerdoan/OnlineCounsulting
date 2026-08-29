using MudBlazor;

namespace OnlineConsulting.Maui.Shared.Layout;

/// <summary>Shell nav: Overview (real) plus roadmap placeholders for unbuilt modules.</summary>
public class CoreUiModule : IUiModule
{
    private const string AdminPrefix = AppRoutes.AdminHome;

    public IReadOnlyList<NavSection> NavSections { get; } =
    [
        new NavSection("Overview", Icons.Material.Outlined.Home, Color.Info,
        [
            new NavItem("Dashboard", AdminPrefix, Icons.Material.Outlined.Dashboard),
        ]),

        new NavSection("Operations", Icons.Material.Outlined.EventNote, Color.Warning,
        [
            new NavItem("Appointments", $"{AdminPrefix}/operations/appointments", Icons.Material.Outlined.Event),
            new NavItem("Work Orders", $"{AdminPrefix}/operations/work-orders", Icons.Material.Outlined.Assignment),
            new NavItem("Equipment", $"{AdminPrefix}/operations/equipment", Icons.Material.Outlined.Build),
        ]),

        new NavSection("Growth", Icons.Material.Outlined.TrendingUp, Color.Success,
        [
            new NavItem("Memberships", $"{AdminPrefix}/growth/memberships", Icons.Material.Outlined.CardMembership),
            new NavItem("Referrals", $"{AdminPrefix}/growth/referrals", Icons.Material.Outlined.Diversity3),
            new NavItem("Promotions", $"{AdminPrefix}/growth/promotions", Icons.Material.Outlined.LocalOffer),
        ]),

        new NavSection("Platform", Icons.Material.Outlined.Business, Color.Secondary,
        [
            new NavItem("Tenants", $"{AdminPrefix}/platform/tenants", Icons.Material.Outlined.Domain, SuperAdminOnly: true),
            new NavItem("Module Catalog", $"{AdminPrefix}/platform/module-offerings", Icons.Material.Outlined.Widgets, SuperAdminOnly: true),
        ]),

        new NavSection("Settings", Icons.Material.Outlined.Settings, Color.Tertiary,
        [
            new NavItem("Users", $"{AdminPrefix}/settings/users", Icons.Material.Outlined.People),
            new NavItem("Invites", $"{AdminPrefix}/settings/invites", Icons.Material.Outlined.MarkEmailUnread),
            new NavItem("Roles", $"{AdminPrefix}/settings/roles", Icons.Material.Outlined.AdminPanelSettings, SuperAdminOnly: true),
            new NavItem("Permissions", $"{AdminPrefix}/settings/permissions", Icons.Material.Outlined.Security, SuperAdminOnly: true),
            new NavItem("Feature Flags", $"{AdminPrefix}/settings/feature-flags", Icons.Material.Outlined.Flag),
        ]),
    ];
}
