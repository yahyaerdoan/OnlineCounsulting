namespace OnlineConsulting.Notifications.Sending;

/// <summary>A push the mock sender recorded instead of sending.</summary>
public record SentPushNotification(Guid UserId, string Title, string Body, IDictionary<string, string>? Data, DateTimeOffset SentAt);
