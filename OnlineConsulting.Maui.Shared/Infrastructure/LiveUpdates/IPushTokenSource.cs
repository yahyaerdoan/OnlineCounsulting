namespace OnlineConsulting.Maui.Shared.Infrastructure.LiveUpdates;

/// <summary>A device push token and the platform the Api's DevicePlatforms expects ("Android" / "iOS").</summary>
public sealed record PushDeviceToken(string Token, string Platform);

/// <summary>Host-provided push provider (e.g. Firebase Cloud Messaging on the MAUI head). Not registered on the web head
/// or before Firebase is configured - PushRegistration then does nothing.</summary>
public interface IPushTokenSource
{
    /// <summary>Raised when the provider rotates this device's token, so it can be re-registered.</summary>
    event Action<PushDeviceToken>? TokenChanged;

    /// <summary>The current token, or null when push is unavailable (permission denied, no Play Services...).</summary>
    Task<PushDeviceToken?> GetTokenAsync(CancellationToken cancellationToken = default);
}
