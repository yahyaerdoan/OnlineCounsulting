using Microsoft.AspNetCore.Components;
using MudBlazor;
using OnlineConsulting.Maui.Shared.Infrastructure.Api;
using OnlineConsulting.Maui.Shared.Infrastructure.Auth;
using OnlineConsulting.Maui.Shared.Infrastructure.LiveUpdates;

namespace OnlineConsulting.Maui.Infrastructure.Auth;

/// <summary>Unregisters this device's push token while the session is still valid, then clears the stored tokens and returns to
/// the storefront home, so no page keeps showing signed-in content and the next user doesn't get this user's notifications.</summary>
public class MauiAuthSession(SecureStorageAccessTokenProvider tokenProvider, MauiAuthenticationStateProvider authStateProvider, PushRegistration pushRegistration, NavigationManager navigation, ISnackbar snackbar) : IAuthSession
{
    public async Task SignOutAsync()
    {
        await pushRegistration.UnregisterAsync();
        await tokenProvider.ClearAsync();
        authStateProvider.SignOut();
        snackbar.ShowSuccess("Goodbye!");
        navigation.NavigateTo("/", replace: true);
    }
}
