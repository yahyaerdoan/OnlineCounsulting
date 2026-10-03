using MudBlazor;

namespace OnlineConsulting.Maui.Shared.Theme;

/// <summary>Single source of truth for the admin app's look; per-tenant palettes will later override just the PaletteLight/PaletteDark colors here (see ITenantThemeProvider).</summary>
public static class AppTheme
{
    public static MudTheme Default { get; } = new()
    {
        PaletteLight = BrandPalette.Light(background: "#F5F5F5"),
        PaletteDark = BrandPalette.Dark(),
        Typography = BrandTypography.Create(),
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "6px",
            DrawerWidthLeft = "240px",
            AppbarHeight = "56px",
        },
    };
}
