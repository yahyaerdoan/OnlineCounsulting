using MudBlazor;

namespace OnlineConsulting.Maui.Shared.Theme;

/// <summary>Marketing site's look - soft white-toned palette, distinct from AppTheme (admin). Route all marketing styling through here.</summary>
public static class MarketingTheme
{
    private static readonly string[] SystemFontStack =
        ["-apple-system", "BlinkMacSystemFont", "Segoe UI", "Helvetica Neue", "Arial", "sans-serif"];

    public static MudTheme Default { get; } = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#C85F49",
            Secondary = "#1D1D1F",
            Tertiary = "#C85F49",
            AppbarBackground = "#FFFFFF",
            AppbarText = "#1D1D1F",
            Background = "#FFFFFF",
            Surface = "#FFFFFF",
            TextPrimary = "#1D1D1F",
            TextSecondary = "#86868B",
            Success = "#1D8A3E",
            Warning = "#B36B00",
            Error = "#D70015",
            Info = "#0071E3",
            LinesDefault = "#D2D2D7",
        },
        Typography = new Typography
        {
            Default = new DefaultTypography { FontFamily = SystemFontStack },
            Body1 = new Body1Typography { FontFamily = SystemFontStack },
            Body2 = new Body2Typography { FontFamily = SystemFontStack },
            Button = new ButtonTypography { FontFamily = SystemFontStack, TextTransform = "none", FontWeight = "400" },
            H1 = new H1Typography { FontFamily = SystemFontStack, FontWeight = "450" },
            H2 = new H2Typography { FontFamily = SystemFontStack, FontWeight = "450" },
            H3 = new H3Typography { FontFamily = SystemFontStack, FontWeight = "450" },
            H4 = new H4Typography { FontFamily = SystemFontStack, FontWeight = "450" },
            H5 = new H5Typography { FontFamily = SystemFontStack, FontWeight = "450" },
            H6 = new H6Typography { FontFamily = SystemFontStack, FontWeight = "450" },
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "8px",
            AppbarHeight = "48px",
        },
    };
}
