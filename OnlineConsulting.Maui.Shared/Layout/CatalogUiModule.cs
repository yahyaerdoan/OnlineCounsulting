using MudBlazor;

namespace OnlineConsulting.Maui.Shared.Layout;

/// <summary>Categories/Services admin nav - split out from CoreUiModule as its own plug-in seam.</summary>
public class CatalogUiModule : IUiModule
{
    private const string AdminPrefix = AppRoutes.AdminHome;

    public IReadOnlyList<NavSection> NavSections { get; } =
    [
        new NavSection("Catalog", Icons.Material.Outlined.Storefront, Color.Primary,
        [
            new NavItem("Categories", $"{AdminPrefix}/catalog/categories", Icons.Material.Outlined.Category),
            new NavItem("Services", $"{AdminPrefix}/catalog/services", Icons.Material.Outlined.Handyman),
        ]),
    ];
}
