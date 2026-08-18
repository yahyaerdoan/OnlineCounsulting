using Microsoft.Extensions.Logging;
using OnlineConsulting.SharedKernel.Notifications;
using System.Collections.Concurrent;

namespace OnlineConsulting.Notifications.Sending;

/// <summary>No network calls - deterministic in-memory sender for dev/testing without a real Firebase project. Default active provider (Push:ActiveProvider defaults to Mock, and Fcm silently falls back to this when Push:FirebaseCredentialsPath isn't configured - see NotificationsServiceCollectionExtensions). Tracks every "sent" notification in a static, process-wide collection so test code can assert on SentNotifications without needing a real push provider.</summary>
public class MockPushNotificationSender(ILogger<MockPushNotificationSender> logger) : IPushNotificationSender
{
    private static readonly ConcurrentBag<SentPushNotification> Sent = [];

    public static IReadOnlyCollection<SentPushNotification> SentNotifications => Sent;

    public Task SendToUserAsync(Guid userId, string title, string body, IDictionary<string, string>? data = null, CancellationToken cancellationToken = default)
    {
        Sent.Add(new SentPushNotification(userId, title, body, data, DateTimeOffset.UtcNow));
        logger.LogInformation("Mock push notification sent to user {UserId}: {Title}", userId, title);

        return Task.CompletedTask;
    }
}

public record SentPushNotification(Guid UserId, string Title, string Body, IDictionary<string, string>? Data, DateTimeOffset SentAt);
