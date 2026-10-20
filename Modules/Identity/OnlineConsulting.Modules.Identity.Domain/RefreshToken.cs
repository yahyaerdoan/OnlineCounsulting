using Core.PersistenceLayer.Repositories.Entities;

namespace OnlineConsulting.Modules.Identity.Domain;

/// <summary>One row per sign-in session; refreshing rewrites only that row, so other devices stay signed in.</summary>
public class RefreshToken : SequentialGuidEntity
{
    public required Guid UserId { get; set; }
    public required string TokenHash { get; set; }
    public required DateTime ExpiresAt { get; set; }
}
