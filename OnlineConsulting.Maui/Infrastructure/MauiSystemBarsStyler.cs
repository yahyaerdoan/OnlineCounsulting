using OnlineConsulting.Maui.Shared.Theme;

namespace OnlineConsulting.Maui.Infrastructure;

/// <summary>MainPage respects the system bar insets (SafeAreaEdges), so its background is what shows behind the status and
/// navigation bars: it takes the app bar color from BrandPalette, and the bar icons switch to dark on light / light on dark.</summary>
public sealed class MauiSystemBarsStyler : ISystemBarsStyler
{
    public void Apply(bool isDarkMode) => MainThread.BeginInvokeOnMainThread(() =>
    {
        var appBar = isDarkMode ? BrandPalette.Dark().AppbarBackground : BrandPalette.Light("#FFFFFF").AppbarBackground;
        var color = Color.FromRgb(appBar.R, appBar.G, appBar.B);

        if (Application.Current?.Windows.FirstOrDefault()?.Page is { } page)
        {
            page.BackgroundColor = color;
        }

        ApplyPlatform(isDarkMode, color);
    });

    private static void ApplyPlatform(bool isDarkMode, Color color)
    {
#if ANDROID
        if (Platform.CurrentActivity?.Window is not { } window)
        {
            return;
        }

        if (AndroidX.Core.View.WindowCompat.GetInsetsController(window, window.DecorView) is { } insetsController)
        {
            insetsController.AppearanceLightStatusBars = !isDarkMode;
            insetsController.AppearanceLightNavigationBars = !isDarkMode;
        }

        if (!OperatingSystem.IsAndroidVersionAtLeast(35))
        {
            window.SetStatusBarColor(Microsoft.Maui.Platform.ColorExtensions.ToPlatform(color));
            window.SetNavigationBarColor(Microsoft.Maui.Platform.ColorExtensions.ToPlatform(color));
        }
#endif
    }
}
