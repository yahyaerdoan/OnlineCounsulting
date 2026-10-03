using Microsoft.AspNetCore.Components;
using MudBlazor;
using OnlineConsulting.Maui.Shared.Infrastructure.Api;
using OnlineConsulting.Maui.Shared.Infrastructure.Auth;

namespace OnlineConsulting.Maui.Infrastructure.Auth;

/// <summary>Clears the stored tokens and returns to the storefront home, so no page keeps showing signed-in content.</summary>
public class MauiAuthSession(SecureStorageAccessTokenProvider tokenProvider, MauiAuthenticationStateProvider authStateProvider, NavigationManager navigation, ISnackbar snackbar) : IAuthSession
{
    public async Task SignOutAsync()
    {
        await tokenProvider.ClearAsync();
        authStateProvider.SignOut();
        snackbar.ShowSuccess("Goodbye!");
        navigation.NavigateTo("/", replace: true);
    }
}
