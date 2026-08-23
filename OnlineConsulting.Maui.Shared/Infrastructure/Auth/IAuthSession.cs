namespace OnlineConsulting.Maui.Shared.Infrastructure.Auth;

/// <summary>Sign-out is host-specific (Web hits a cookie-clearing HTTP endpoint, MAUI clears its
/// stored token and flips its in-memory AuthenticationStateProvider), so MainLayout's sign-out menu
/// item calls this instead of a hardcoded Href.</summary>
public interface IAuthSession
{
    Task SignOutAsync();
}
