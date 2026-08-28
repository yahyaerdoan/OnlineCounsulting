using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Usage: await DialogService.ConfirmDeleteAsync("user", "Jane Doe").</summary>
public static class DialogServiceExtensions
{
    /// <summary>The MaxWidth.Small/FullWidth shape every admin CRUD dialog uses.</summary>
    public static readonly DialogOptions SmallDialogOptions = new() { MaxWidth = MaxWidth.Small, FullWidth = true };

    public static async Task<bool> ConfirmDeleteAsync(this IDialogService dialogService, string entityLabel, string itemName)
    {
        // YesText defaults to "OK" - MudMessageBox already renders it Color.Primary.
        var confirmed = await dialogService.ShowMessageBoxAsync("Delete Confirmation", $"Delete {entityLabel} \"{itemName}\"? This cannot be undone.", cancelText: "Cancel");
        return confirmed == true;
    }

    /// <summary>Generic yes/no confirm for non-delete actions.</summary>
    public static async Task<bool> ConfirmAsync(this IDialogService dialogService, string title, string message)
    {
        var confirmed = await dialogService.ShowMessageBoxAsync(title, message, cancelText: "Cancel");
        return confirmed == true;
    }

    /// <summary>Opens a parameterless CRUD dialog (create-style) and reloads the table unless cancelled.</summary>
    public static Task ShowAndReloadAsync<TDialog>(this IDialogService dialogService, string title, Func<Task> reload, DialogOptions? options = null)
        where TDialog : IComponent =>
        ShowAndReloadAsync(dialogService.ShowAsync<TDialog>(title, options ?? SmallDialogOptions), reload);

    /// <summary>Opens a parameterized CRUD dialog (edit-style) and reloads the table unless cancelled.</summary>
    public static Task ShowAndReloadAsync<TDialog>(this IDialogService dialogService, string title, DialogParameters<TDialog> parameters, Func<Task> reload, DialogOptions? options = null)
        where TDialog : IComponent =>
        ShowAndReloadAsync(dialogService.ShowAsync<TDialog>(title, parameters, options ?? SmallDialogOptions), reload);

    private static async Task ShowAndReloadAsync(Task<IDialogReference> showDialog, Func<Task> reload)
    {
        var dialog = await showDialog;
        var result = await dialog.Result;
        if (result is { Canceled: false })
        {
            await reload();
        }
    }
}
