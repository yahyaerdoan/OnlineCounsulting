using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Forms;

/// <summary>Unsent text for this app session (sessionStorage): survives leaving the page, gone once the app or tab is closed.
/// Keyed per signed-in user, so the next account on a shared device never sees it.</summary>
public sealed class DraftStore(IJSRuntime jsRuntime, AuthenticationStateProvider authStateProvider)
{
    private const string KeyPrefix = "comfortpro-draft:";

    public async ValueTask<string?> GetAsync(string key) => await jsRuntime.InvokeAsync<string?>("sessionStorage.getItem", await StorageKeyAsync(key));

    /// <summary>Empty text removes the draft.</summary>
    public async ValueTask SaveAsync(string key, string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            await RemoveAsync(key);
            return;
        }

        await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", await StorageKeyAsync(key), text);
    }

    public async ValueTask RemoveAsync(string key) => await jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", await StorageKeyAsync(key));

    private async Task<string> StorageKeyAsync(string key)
    {
        var user = (await authStateProvider.GetAuthenticationStateAsync()).User;
        return $"{KeyPrefix}{user.FindFirst(ClaimTypes.NameIdentifier)?.Value}:{key}";
    }
}
