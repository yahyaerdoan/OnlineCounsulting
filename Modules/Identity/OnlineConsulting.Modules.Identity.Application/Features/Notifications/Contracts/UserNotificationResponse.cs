using Hateoas;
using System.Text.Json;
using OnlineConsulting.Modules.Identity.Domain;

namespace OnlineConsulting.Modules.Identity.Application.Features.Notifications.Contracts;

/// <summary>Data holds the push payload keys (appointmentId, orderId...) so the client can open the related screen.</summary>
public record UserNotificationResponse(Guid Id, string Title, string Body, IReadOnlyDictionary<string, string> Data, DateTimeOffset CreatedAt, bool IsRead) : LinkedRecord
{
    public static UserNotificationResponse FromDomain(UserNotification notification) => new(
        notification.Id,
        notification.Title,
        notification.Body,
        string.IsNullOrWhiteSpace(notification.DataJson)
            ? new Dictionary<string, string>()
            : JsonSerializer.Deserialize<Dictionary<string, string>>(notification.DataJson) ?? [],
        notification.CreatedAt,
        notification.ReadAt is not null);
}
