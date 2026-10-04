using Hateoas;

namespace OnlineConsulting.Api.Features.Referrals;

/// <summary>A referral in the staff list, with the referrer's and referred user's email and user name from Identity.</summary>
public record AdminReferralResponse(Guid Id, string Code, string Status, decimal? RewardAmount, DateTimeOffset? RewardedAt, Guid ReferrerUserId,
    string? ReferrerEmail, string? ReferrerName, Guid ReferredUserId, string? ReferredEmail, string? ReferredName) : LinkedRecord;
