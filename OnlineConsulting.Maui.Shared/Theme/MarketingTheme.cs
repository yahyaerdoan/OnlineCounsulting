using MudBlazor;

namespace OnlineConsulting.Maui.Shared.Theme;

/// <summary>Marketing site theme: same BrandPalette and type scale as AppTheme, on a pure-white page with a compact app bar. Route all marketing styling through here.</summary>
public static class MarketingTheme
{
    public static MudTheme Default { get; } = new()
    {
        PaletteLight = BrandPalette.Light(background: "#FFFFFF"),
        PaletteDark = BrandPalette.Dark(),
        Typography = BrandTypography.Create(),
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "6px",
            AppbarHeight = "48px",
        },
    };
}
