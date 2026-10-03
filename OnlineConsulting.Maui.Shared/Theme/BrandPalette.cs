using MudBlazor;

namespace OnlineConsulting.Maui.Shared.Theme;

/// <summary>Single source of the brand colors for AppTheme (admin) and MarketingTheme, modeled on Microsoft's Fluent 2 palette: one brand blue, everything else neutral gray. Secondary is deliberately a neutral - the UI uses Color.Secondary for muted text and icons. Every text/background pair meets WCAG AA (4.5:1) in both modes.</summary>
public static class BrandPalette
{
    public const string Brand = "#0F6CBD";
    public const string BrandOnDark = "#479EF5";
    public const string MutedText = "#616161";
    public const string MutedTextOnDark = "#D6D6D6";

    /// <summary>Segoe UI on Windows, the platform UI font elsewhere - the same stack Microsoft's own sites fall back through.</summary>
    public static readonly string[] FontStack =
        ["Segoe UI Variable Text", "Segoe UI", "-apple-system", "BlinkMacSystemFont", "Roboto", "Helvetica Neue", "Arial", "sans-serif"];

    /// <summary>Light palette with the given page background, so the marketing site keeps a pure-white page while the admin keeps a light-gray canvas.</summary>
    public static PaletteLight Light(string background) => new()
    {
        Primary = Brand,
        Secondary = MutedText,
        Tertiary = "#0E7C7B",
        AppbarBackground = "#FFFFFF",
        AppbarText = "#242424",
        Background = background,
        BackgroundGray = "#F5F5F5",
        Surface = "#FFFFFF",
        DrawerBackground = "#FAFAFA",
        DrawerText = "#242424",
        DrawerIcon = "#616161",
        TextPrimary = "#242424",
        TextSecondary = MutedText,
        ActionDefault = "#424242",
        Success = "#107C10",
        Warning = "#BC4B09",
        Error = "#C50F1F",
        Info = Brand,
        LinesDefault = "#E0E0E0",
        LinesInputs = "#8A8A8A",
        TableLines = "#E0E0E0",
        Divider = "#E0E0E0",
    };

    /// <summary>Dark palette shared by both themes; the brand blue is lightened so it keeps AA contrast on the dark surfaces.</summary>
    public static PaletteDark Dark() => new()
    {
        Primary = BrandOnDark,
        Secondary = MutedTextOnDark,
        Tertiary = "#2DD4BF",
        AppbarBackground = "#1F1F1F",
        AppbarText = "#FFFFFF",
        Background = "#141414",
        BackgroundGray = "#1F1F1F",
        Surface = "#292929",
        DrawerBackground = "#1F1F1F",
        DrawerText = "#FFFFFF",
        DrawerIcon = "#D6D6D6",
        TextPrimary = "#FFFFFF",
        TextSecondary = MutedTextOnDark,
        ActionDefault = "#D6D6D6",
        Success = "#54B054",
        Warning = "#F7C356",
        Error = "#F1707B",
        Info = BrandOnDark,
        LinesDefault = "#3D3D3D",
        LinesInputs = "#8A8A8A",
        TableLines = "#3D3D3D",
        Divider = "#3D3D3D",
    };
}
