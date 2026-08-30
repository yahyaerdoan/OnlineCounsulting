using MudBlazor;

namespace OnlineConsulting.Maui.Shared.Layout;

/// <summary>Orders admin nav - split out from CoreUiModule as its own plug-in seam.</summary>
public class CommerceUiModule : IUiModule
{
    private const string AdminPrefix = AppRoutes.AdminHome;

    public IReadOnlyList<NavSection> NavSections { get; } =
    [
        new NavSection("Commerce", Icons.Material.Outlined.ShoppingCart, Color.Primary,
        [
            new NavItem("Orders", $"{AdminPrefix}/commerce/orders", Icons.Material.Outlined.Receipt),
        ]),
    ];
}
