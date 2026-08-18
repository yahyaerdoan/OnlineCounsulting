using OnlineConsulting.Modules.Referrals.Domain;

namespace OnlineConsulting.Modules.Referrals.Application.Features.Referrals.Contracts;

public record ReferralResponse(Guid Id, Guid ReferrerUserId, Guid ReferredUserId, string Code, string Status, decimal? RewardAmount, DateTimeOffset? RewardedAt)
{
    public static ReferralResponse FromDomain(Referral entity) => new(entity.Id, entity.ReferrerUserId, entity.ReferredUserId, entity.Code, entity.Status, entity.RewardAmount, entity.RewardedAt);
}
