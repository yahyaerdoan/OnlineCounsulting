using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Referrals.Domain;

/// <summary>One per user, created lazily on first request. Plain UserId, no navigation - User lives in the Identity module's own DbContext.</summary>
public class ReferralCode : TenantEntity<Guid>
{
    public required Guid UserId { get; set; }
    public required string Code { get; set; }
}
