using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Identity.Domain;

/// <summary>A teammate invite; Token alone resolves TenantId on acceptance, so it's exempt from the EF tenant query filter (see AppIdentityDbContext) since AcceptInvite is anonymous.</summary>
public class Invite : SequentialGuidTenantEntity
{
    public required string Email { get; set; }
    public required string Token { get; set; }
    public required string RoleName { get; set; }
    public string Status { get; set; } = InviteStatuses.Pending;
    public required DateTime ExpiresAt { get; set; }
    public required Guid InvitedByUserId { get; set; }
    public DateTime? AcceptedAt { get; set; }
}
