using OnlineConsulting.Modules.Referrals.Domain;
using OnlineConsulting.SharedKernel.Identity;

namespace OnlineConsulting.Modules.Referrals.Application.Features.Referrals.Contracts;

/// <summary>A referral in the staff list, with the referrer's and referred user's email and user name.</summary>
public record AdminReferralResponse(Guid Id, string Code, string Status, decimal? RewardAmount, DateTimeOffset? RewardedAt, Guid ReferrerUserId,
    string? ReferrerEmail, string? ReferrerName, Guid ReferredUserId, string? ReferredEmail, string? ReferredName)
{
    public static AdminReferralResponse FromDomain(Referral referral, UserContact? referrer, UserContact? referred) => new(
        referral.Id, referral.Code, referral.Status, referral.RewardAmount, referral.RewardedAt,
        referral.ReferrerUserId, referrer?.Email, referrer?.UserName, referral.ReferredUserId, referred?.Email, referred?.UserName);
}
