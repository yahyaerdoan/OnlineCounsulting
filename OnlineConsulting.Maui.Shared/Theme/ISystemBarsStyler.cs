namespace OnlineConsulting.Maui.Shared.Theme;

/// <summary>Native status/navigation bar styling, registered only by the MAUI head: the bars take the app bar's color and
/// light-or-dark icons for the active theme, the way Instagram/Facebook/LinkedIn blend their system bars into the app.</summary>
public interface ISystemBarsStyler
{
    void Apply(bool isDarkMode);
}
