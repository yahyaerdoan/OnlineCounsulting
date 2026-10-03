namespace OnlineConsulting.Maui.Shared.Infrastructure;

/// <summary>An icon for a saved address from its name (Home, Work, Rental...), so address cards read at a glance.</summary>
public static class AddressIcons
{
    public static string For(string name) => name.ToLowerInvariant() switch
    {
        var n when n.Contains("home") || n.Contains("house") => MudBlazor.Icons.Material.Outlined.Home,
        var n when n.Contains("work") || n.Contains("office") || n.Contains("business") => MudBlazor.Icons.Material.Outlined.Business,
        var n when n.Contains("rent") || n.Contains("apartment") || n.Contains("condo") => MudBlazor.Icons.Material.Outlined.Apartment,
        _ => MudBlazor.Icons.Material.Outlined.LocationOn,
    };
}
