using MudBlazor;

namespace OnlineConsulting.Maui.Shared.Theme;

/// <summary>Single source of truth for the admin app's look. Per-tenant palettes will later
/// override just the PaletteLight/PaletteDark colors here (see ITenantThemeProvider), so keep
/// every visual choice routed through this theme instead of ad-hoc component styling.</summary>
public static class AppTheme
{
    public static MudTheme Default { get; } = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#4F46E5",
            Secondary = "#0EA5A4",
            Tertiary = "#F59E0B",
            AppbarBackground = "#FFFFFF",
            AppbarText = "#1E1B4B",
            Background = "#F7F7FB",
            Surface = "#FFFFFF",
            DrawerBackground = "#FFFFFF",
            DrawerText = "#33334D",
            TextPrimary = "#1E1B2E",
            TextSecondary = "#5F5F72",
            Success = "#16A34A",
            Warning = "#F59E0B",
            Error = "#DC2626",
            Info = "#0284C7",
            LinesDefault = "#E5E5F0",
        },
        PaletteDark = new PaletteDark
        {
            Primary = "#818CF8",
            Secondary = "#2DD4BF",
            Tertiary = "#FBBF24",
            AppbarBackground = "#16161F",
            AppbarText = "#F4F4FA",
            Background = "#101018",
            Surface = "#181822",
            DrawerBackground = "#181822",
            DrawerText = "#D7D7E8",
            TextPrimary = "#F4F4FA",
            TextSecondary = "#A6A6BF",
            Success = "#4ADE80",
            Warning = "#FBBF24",
            Error = "#F87171",
            Info = "#38BDF8",
            LinesDefault = "#2A2A38",
        },
        Typography = new Typography
        {
            Default = new DefaultTypography { FontFamily = ["Inter", "Segoe UI", "Helvetica", "Arial", "sans-serif"] },
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "10px",
            DrawerWidthLeft = "240px",
            AppbarHeight = "64px",
        },
    };
}
