using Core.PersistenceLayer.Repositories.Entities;

namespace OnlineConsulting.Modules.Identity.Domain;

/// <summary>A mobile device's push-notification token (FCM). Token is globally unique - re-registering the same device (even under a different account, e.g. after logout/login) updates UserId in place rather than creating a duplicate row.</summary>
public class DeviceToken : Entity<Guid>
{
    public required Guid UserId { get; set; }
    public required string Token { get; set; }
    public required string Platform { get; set; }
    public required DateTimeOffset RegisteredAt { get; set; }
}
