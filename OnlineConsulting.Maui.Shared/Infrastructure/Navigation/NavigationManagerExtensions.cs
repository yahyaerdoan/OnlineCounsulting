using Microsoft.AspNetCore.Components;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Navigation;

/// <summary>Usage: Navigation.ClearQueryFlag("welcome") to consume a one-shot ?welcome=true marker.
/// On a static-SSR page, use WithoutQueryFlag + history.replaceState instead (NavigateTo reloads).</summary>
public static class NavigationManagerExtensions
{
    /// <summary>Returns the URI with the marker removed, or null if it wasn't present.</summary>
    public static string? WithoutQueryFlag(this NavigationManager navigation, string flag)
    {
        var marker = $"{flag}=true";
        return !navigation.Uri.Contains(marker, StringComparison.Ordinal)
            ? null
            : navigation.Uri
            .Replace($"?{marker}&", "?", StringComparison.Ordinal)
            .Replace($"&{marker}", "", StringComparison.Ordinal)
            .Replace($"?{marker}", "", StringComparison.Ordinal);
    }

    public static void ClearQueryFlag(this NavigationManager navigation, string flag)
    {
        var cleanUri = navigation.WithoutQueryFlag(flag);
        if (cleanUri is not null)
        {
            navigation.NavigateTo(cleanUri, replace: true);
        }
    }
}
