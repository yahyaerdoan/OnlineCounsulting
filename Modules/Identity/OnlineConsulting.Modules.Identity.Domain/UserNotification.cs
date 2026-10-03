using Core.PersistenceLayer.Repositories.Entities;

namespace OnlineConsulting.Modules.Identity.Domain;

/// <summary>One entry in a user's in-app notification inbox. DataJson carries the same key/values as the push payload
/// (e.g. appointmentId, orderId) so clients can deep-link; ReadAt null means unread.</summary>
public class UserNotification : SequentialGuidEntity
{
    public required Guid UserId { get; set; }
    public required string Title { get; set; }
    public required string Body { get; set; }
    public string? DataJson { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ReadAt { get; set; }
}
