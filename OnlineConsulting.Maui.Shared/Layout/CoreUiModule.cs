using MudBlazor;

namespace OnlineConsulting.Maui.Shared.Layout;

/// <summary>Shell nav: Overview (real) plus roadmap placeholders for unbuilt modules.</summary>
public class CoreUiModule : IUiModule
{
    private const string AdminPrefix = AppRoutes.AdminHome;

    public IReadOnlyList<NavSection> NavSections { get; } =
    [
        new NavSection("Overview", Icons.Material.Filled.Home, Color.Info,
        [
            new NavItem("Dashboard", AdminPrefix, Icons.Material.Filled.Dashboard),
        ]),

        new NavSection("Catalog", Icons.Material.Filled.Storefront, Color.Primary,
        [
            new NavItem("Categories", $"{AdminPrefix}/catalog/categories", Icons.Material.Filled.Category),
            new NavItem("Services", $"{AdminPrefix}/catalog/services", Icons.Material.Filled.Handyman),
        ]),

        new NavSection("Operations", Icons.Material.Filled.EventNote, Color.Warning,
        [
            new NavItem("Appointments", $"{AdminPrefix}/operations/appointments", Icons.Material.Filled.Event),
            new NavItem("Work Orders", $"{AdminPrefix}/operations/work-orders", Icons.Material.Filled.Assignment),
            new NavItem("Equipment", $"{AdminPrefix}/operations/equipment", Icons.Material.Filled.Build),
        ]),

        new NavSection("Growth", Icons.Material.Filled.TrendingUp, Color.Success,
        [
            new NavItem("Memberships", $"{AdminPrefix}/growth/memberships", Icons.Material.Filled.CardMembership),
            new NavItem("Referrals", $"{AdminPrefix}/growth/referrals", Icons.Material.Filled.Diversity3),
            new NavItem("Promotions", $"{AdminPrefix}/growth/promotions", Icons.Material.Filled.LocalOffer),
        ]),

        new NavSection("Platform", Icons.Material.Filled.Business, Color.Secondary,
        [
            new NavItem("Tenants", $"{AdminPrefix}/platform/tenants", Icons.Material.Filled.Domain, SuperAdminOnly: true),
            new NavItem("Module Catalog", $"{AdminPrefix}/platform/module-offerings", Icons.Material.Filled.Widgets, SuperAdminOnly: true),
        ]),

        new NavSection("Settings", Icons.Material.Filled.Settings, Color.Tertiary,
        [
            new NavItem("Users", $"{AdminPrefix}/settings/users", Icons.Material.Filled.People),
            new NavItem("Invites", $"{AdminPrefix}/settings/invites", Icons.Material.Filled.MarkEmailUnread),
            new NavItem("Roles", $"{AdminPrefix}/settings/roles", Icons.Material.Filled.AdminPanelSettings, SuperAdminOnly: true),
            new NavItem("Permissions", $"{AdminPrefix}/settings/permissions", Icons.Material.Filled.Security, SuperAdminOnly: true),
            new NavItem("Feature Flags", $"{AdminPrefix}/settings/feature-flags", Icons.Material.Filled.Flag),
        ]),
    ];
}
