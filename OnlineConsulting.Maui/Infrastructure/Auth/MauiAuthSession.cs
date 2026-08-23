using MudBlazor;
using OnlineConsulting.Maui.Shared.Infrastructure.Api;
using OnlineConsulting.Maui.Shared.Infrastructure.Auth;

namespace OnlineConsulting.Maui.Infrastructure.Auth;

public class MauiAuthSession(SecureStorageAccessTokenProvider tokenProvider, MauiAuthenticationStateProvider authStateProvider, ISnackbar snackbar) : IAuthSession
{
    public async Task SignOutAsync()
    {
        await tokenProvider.ClearAsync();
        authStateProvider.SignOut();
        snackbar.ShowSuccess("Goodbye!");
    }
}
