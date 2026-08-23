using Microsoft.AspNetCore.Components;
using OnlineConsulting.Maui.Shared.Infrastructure.Auth;
using OnlineConsulting.Maui.Shared.Layout;

namespace OnlineConsulting.Maui.Web.Infrastructure.Auth;

/// <summary>Signing out means clearing the auth cookie, which needs a real HTTP response - the
/// AppRoutes.Logout minimal endpoint in Program.cs does that, so this just forces a full-page
/// request to it instead of an interactive-circuit navigation.</summary>
public class WebAuthSession(NavigationManager navigation) : IAuthSession
{
    public Task SignOutAsync()
    {
        navigation.NavigateTo(AppRoutes.Logout, forceLoad: true);
        return Task.CompletedTask;
    }
}
