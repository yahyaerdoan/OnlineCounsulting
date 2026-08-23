namespace OnlineConsulting.Maui.Shared.Infrastructure.Auth;

/// <summary>Decouples BearerTokenHandler from IAuthSession (avoids a DI cycle); MainLayout subscribes.</summary>
public class AuthenticationExpiredNotifier
{
    public event Action? AuthenticationExpired;

    public void NotifyExpired() => AuthenticationExpired?.Invoke();
}
