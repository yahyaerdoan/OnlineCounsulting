using Microsoft.Extensions.Logging;
using OnlineConsulting.SharedKernel.Notifications;
using System.Collections.Concurrent;

namespace OnlineConsulting.Notifications.Sending;

/// <summary>In-memory sender for dev/testing; default provider, tracks sends in a static collection so tests can assert on SentNotifications.</summary>
public class MockPushNotificationSender(ILogger<MockPushNotificationSender> logger) : IPushNotificationSender
{
    private static readonly ConcurrentBag<SentPushNotification> _sent = [];

    public static IReadOnlyCollection<SentPushNotification> SentNotifications => _sent;

    public Task SendToUserAsync(Guid userId, string title, string body, IDictionary<string, string>? data = null, CancellationToken cancellationToken = default)
    {
        _sent.Add(new SentPushNotification(userId, title, body, data, DateTimeOffset.UtcNow));

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Mock push notification sent to user {UserId}: {Title}", userId, title);
        }

        return Task.CompletedTask;
    }
}

public record SentPushNotification(Guid UserId, string Title, string Body, IDictionary<string, string>? Data, DateTimeOffset SentAt);
