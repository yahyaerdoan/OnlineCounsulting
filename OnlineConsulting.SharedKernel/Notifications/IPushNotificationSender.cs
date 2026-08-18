namespace OnlineConsulting.SharedKernel.Notifications;

/// <summary>Push (not email/SignalR) - reaches a user's mobile device even when the app isn't open, which SignalR (only reaches actively-connected clients) can't. Sends to every device the user has registered (IDeviceTokenRepository.GetTokensForUserAsync) - implemented in OnlineConsulting.Notifications via Firebase Cloud Messaging, wired the same keyed-provider way as IPaymentGateway/IStorageService.</summary>
public interface IPushNotificationSender
{
    Task SendToUserAsync(Guid userId, string title, string body, IDictionary<string, string>? data = null, CancellationToken cancellationToken = default);
}
