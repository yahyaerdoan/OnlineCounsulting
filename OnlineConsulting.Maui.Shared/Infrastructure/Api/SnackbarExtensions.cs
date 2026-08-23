using MudBlazor;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Usage: Snackbar.ShowError(message) / Snackbar.ShowSuccess(message).</summary>
public static class SnackbarExtensions
{
    public static void ShowError(this ISnackbar snackbar, string message) =>
        snackbar.Add(message, Severity.Error, options => options.SnackbarVariant = Variant.Outlined);

    public static void ShowSuccess(this ISnackbar snackbar, string message) =>
        snackbar.Add(message, Severity.Success, options => options.SnackbarVariant = Variant.Outlined);
}
