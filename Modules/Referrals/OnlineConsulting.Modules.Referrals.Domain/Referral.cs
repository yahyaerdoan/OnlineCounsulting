using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Referrals.Domain;

/// <summary>A user referred with another user's code; Pending until it is rewarded once. A user can be referred at most once.</summary>
public class Referral : SequentialGuidTenantEntity
{
    private Referral()
    {
    }

    public Guid ReferrerUserId { get; private set; }
    public Guid ReferredUserId { get; private set; }
    public string Code { get; private set; } = string.Empty;

    /// <summary>One of <see cref="ReferralStatuses"/>.</summary>
    public string Status { get; private set; } = ReferralStatuses.Pending;

    public decimal? RewardAmount { get; private set; }
    public DateTimeOffset? RewardedAt { get; private set; }

    public bool IsRewarded => Status == ReferralStatuses.Rewarded;

    /// <summary>Records a pending referral; a user cannot refer themselves.</summary>
    public static Referral Create(Guid referrerUserId, Guid referredUserId, string code)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        if (referrerUserId == referredUserId)
        {
            throw new ArgumentException("A user cannot refer themselves.", nameof(referredUserId));
        }

        return new Referral { ReferrerUserId = referrerUserId, ReferredUserId = referredUserId, Code = code };
    }

    /// <summary>Rewards the referrer once. Requires the referral not to be <see cref="IsRewarded"/>.</summary>
    public void Reward(decimal amount, DateTimeOffset rewardedAt)
    {
        if (IsRewarded)
        {
            throw new InvalidOperationException($"Referral {Id} is already rewarded.");
        }

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
        Status = ReferralStatuses.Rewarded;
        RewardAmount = amount;
        RewardedAt = rewardedAt;
    }
}
