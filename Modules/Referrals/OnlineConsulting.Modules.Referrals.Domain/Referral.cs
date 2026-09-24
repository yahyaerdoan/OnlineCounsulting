using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Referrals.Domain;

/// <summary>Status moves Pending -> Rewarded only, via CompleteReferralCommand; ReferredUserId is unique so a user can be referred at most once (see ReferralsDbContext).</summary>
public class Referral : SequentialGuidTenantEntity
{
    public required Guid ReferrerUserId { get; set; }
    public required Guid ReferredUserId { get; set; }
    public required string Code { get; set; }
    public required string Status { get; set; }
    public decimal? RewardAmount { get; set; }
    public DateTimeOffset? RewardedAt { get; set; }
}
