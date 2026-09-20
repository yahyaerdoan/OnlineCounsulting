namespace OnlineConsulting.SharedKernel.Notifications;

/// <summary>Push (not email/SignalR) - reaches a user's device even when the app isn't open, unlike SignalR which only reaches actively-connected clients.</summary>
public interface IPushNotificationSender
{
    Task SendToUserAsync(Guid userId, string title, string body, IDictionary<string, string>? data = null, CancellationToken cancellationToken = default);
}
