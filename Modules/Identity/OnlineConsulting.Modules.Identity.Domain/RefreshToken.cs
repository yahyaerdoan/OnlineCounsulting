using Core.PersistenceLayer.Repositories.Entities;

namespace OnlineConsulting.Modules.Identity.Domain;

/// <summary>One active row per user; IssueAsync overwrites it in place, which is what gives token rotation.</summary>
public class RefreshToken : Entity<Guid>
{
    public required Guid UserId { get; set; }
    public required string TokenHash { get; set; }
    public required DateTime ExpiresAt { get; set; }
}
