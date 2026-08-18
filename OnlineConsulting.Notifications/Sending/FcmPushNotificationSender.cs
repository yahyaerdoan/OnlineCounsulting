using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;
using OnlineConsulting.SharedKernel.Notifications;

namespace OnlineConsulting.Notifications.Sending;

/// <summary>Structurally complete but untested end-to-end - like PayPalPaymentGateway, wiring this fully needs a real Firebase project's service account credentials, which aren't configured in this environment. Sends one message per registered device rather than a single multicast call, so one dead/unregistered token doesn't fail the whole batch for a user with multiple devices.</summary>
public class FcmPushNotificationSender(IDeviceTokenRepository deviceTokenRepository, ILogger<FcmPushNotificationSender> logger) : IPushNotificationSender
{
    public async Task SendToUserAsync(Guid userId, string title, string body, IDictionary<string, string>? data = null, CancellationToken cancellationToken = default)
    {
        var tokens = await deviceTokenRepository.GetTokensForUserAsync(userId, cancellationToken);
        if (tokens.Count == 0)
            return;

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
                await FirebaseMessaging.DefaultInstance.SendAsync(message, cancellationToken);
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
