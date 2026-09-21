namespace OnlineConsulting.Maui.Shared.Infrastructure.Auth;

/// <summary>Sign-out is host-specific (Web clears a cookie via HTTP, MAUI clears its stored token), so MainLayout calls this instead of a hardcoded Href.</summary>
public interface IAuthSession
{
    Task SignOutAsync();
}
