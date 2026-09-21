using Microsoft.AspNetCore.Components;
using OnlineConsulting.Maui.Shared.Infrastructure.Auth;
using OnlineConsulting.Maui.Shared.Layout;

namespace OnlineConsulting.Maui.Web.Infrastructure.Auth;

/// <summary>Clearing the auth cookie needs a real HTTP response, so this forces a full-page request to AppRoutes.Logout instead of an in-circuit navigation.</summary>
public class WebAuthSession(NavigationManager navigation) : IAuthSession
{
    public Task SignOutAsync()
    {
        navigation.NavigateTo(AppRoutes.Logout, forceLoad: true);

        return Task.CompletedTask;
    }
}
