using MudBlazor;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Usage: await DialogService.ConfirmDeleteAsync("user", "Jane Doe").</summary>
public static class DialogServiceExtensions
{
    public static async Task<bool> ConfirmDeleteAsync(this IDialogService dialogService, string entityLabel, string itemName)
    {
        // YesText defaults to "OK" - MudMessageBox already renders it Color.Primary.
        var confirmed = await dialogService.ShowMessageBoxAsync(
            "Delete Confirmation", $"Delete {entityLabel} \"{itemName}\"? This cannot be undone.", cancelText: "Cancel");
        return confirmed == true;
    }
}
