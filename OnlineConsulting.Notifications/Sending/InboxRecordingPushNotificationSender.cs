using Microsoft.Extensions.Logging;
using OnlineConsulting.SharedKernel.Notifications;

namespace OnlineConsulting.Notifications.Sending;

/// <summary>Records every notification in the user's in-app inbox (the bell), then hands it to the active push provider.
/// The inbox is the source of truth - push is only a delivery channel - so a failed inbox write is logged and the push
/// still goes out.</summary>
public sealed class InboxRecordingPushNotificationSender(IPushNotificationSender pushProvider, IUserNotificationInbox? inbox,
    ILogger<InboxRecordingPushNotificationSender> logger) : IPushNotificationSender
{
    public async Task SendToUserAsync(Guid userId, string title, string body, IDictionary<string, string>? data = null, CancellationToken cancellationToken = default)
    {
        if (inbox is not null)
        {
            try
            {
                await inbox.AddAsync(userId, title, body, data, cancellationToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                logger.LogWarning(exception, "Recording notification '{Title}' in user {UserId}'s inbox failed.", title, userId);
            }
        }

        await pushProvider.SendToUserAsync(userId, title, body, data, cancellationToken);
    }
}
