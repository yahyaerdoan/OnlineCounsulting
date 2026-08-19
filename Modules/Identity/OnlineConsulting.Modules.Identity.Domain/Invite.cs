using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Identity.Domain;

/// <summary>A tenant admin's invitation for a new teammate to join their tenant. The Token is the only thing
/// the invited person needs - it is embedded in the emailed link and resolves TenantId automatically on
/// acceptance, so they never see or enter a tenant id themselves. Not tenant-scoped by an EF query filter
/// (see AppIdentityDbContext) because AcceptInvite is anonymous and must be able to look a row up by Token
/// alone, before any tenant is known.</summary>
public class Invite : TenantEntity<Guid>
{
    public required string Email { get; set; }
    public required string Token { get; set; }
    public required string RoleName { get; set; }
    public string Status { get; set; } = InviteStatuses.Pending;
    public required DateTime ExpiresAt { get; set; }
    public required Guid InvitedByUserId { get; set; }
    public DateTime? AcceptedAt { get; set; }
}
