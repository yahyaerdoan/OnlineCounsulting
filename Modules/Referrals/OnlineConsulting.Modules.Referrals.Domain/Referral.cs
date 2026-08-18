using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Referrals.Domain;

public class Referral : TenantEntity<Guid>
{
    public required Guid ReferrerUserId { get; set; }
    public required Guid ReferredUserId { get; set; }
    public required string Code { get; set; }
    public required string Status { get; set; }
    public decimal? RewardAmount { get; set; }
    public DateTimeOffset? RewardedAt { get; set; }
}
