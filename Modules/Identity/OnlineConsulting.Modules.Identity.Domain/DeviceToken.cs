using Core.PersistenceLayer.Repositories.Entities;

namespace OnlineConsulting.Modules.Identity.Domain;

/// <summary>A mobile device's FCM push token; globally unique, so re-registering the same device updates UserId in place.</summary>
public class DeviceToken : SequentialGuidEntity
{
    public required Guid UserId { get; set; }
    public required string Token { get; set; }
    public required string Platform { get; set; }
    public required DateTimeOffset RegisteredAt { get; set; }
}
