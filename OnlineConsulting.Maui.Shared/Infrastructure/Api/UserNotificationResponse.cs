using OnlineConsulting.Maui.Shared.Infrastructure.Hateoas;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors GET /api/v1/notifications items. Data carries the push payload keys (appointmentId, orderId...).</summary>
public record UserNotificationResponse(Guid Id, string Title, string Body, Dictionary<string, string> Data, DateTimeOffset CreatedAt, bool IsRead) : HalResource;
