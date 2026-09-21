using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;
using OnlineConsulting.SharedKernel.Notifications;

namespace OnlineConsulting.Notifications.Sending;

/// <summary>Untested end-to-end - needs real Firebase credentials. Sends one message per device so a dead token doesn't fail the whole batch.</summary>
public class FcmPushNotificationSender(IDeviceTokenRepository deviceTokenRepository, ILogger<FcmPushNotificationSender> logger) : IPushNotificationSender
{
    public async Task SendToUserAsync(Guid userId, string title, string body, IDictionary<string, string>? data = null, CancellationToken cancellationToken = default)
    {
        var tokens = await deviceTokenRepository.GetTokensForUserAsync(userId, cancellationToken);
        if (tokens.Count == 0)
        {
            return;
        }

        foreach (var token in tokens)
        {
            var message = new Message
            {
                Token = token,
                Notification = new Notification { Title = title, Body = body },
                Data = data is null ? null : new Dictionary<string, string>(data),
            };

            try
            {
                _ = await FirebaseMessaging.DefaultInstance.SendAsync(message, cancellationToken);
            }
            catch (FirebaseMessagingException ex) when (ex.MessagingErrorCode is MessagingErrorCode.Unregistered)
            {
                await deviceTokenRepository.RemoveAsync(token, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Push notification delivery failed for a device token belonging to user {UserId}.", userId);
            }
        }
    }
}
