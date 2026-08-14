using Core.PersistenceLayer.Repositories.Entities;

namespace OnlineConsulting.Modules.Identity.Domain;

// One active row per user - IssueAsync overwrites the existing row instead of inserting a new
// one, which is what gives rotation (the previous token stops matching TokenHash and is rejected).
public class RefreshToken : Entity<Guid>
{
    public required Guid UserId { get; set; }
    public required string TokenHash { get; set; }
    public required DateTime ExpiresAt { get; set; }
}
