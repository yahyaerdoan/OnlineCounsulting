namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/referrals/query's response shape (Api-layer AdminReferralResponse, enriched with user display names).</summary>
public record ReferralResponse(Guid Id, string Code, string Status, decimal? RewardAmount, DateTimeOffset? RewardedAt, Guid ReferrerUserId, string? ReferrerEmail, string? ReferrerName, Guid ReferredUserId, string? ReferredEmail, string? ReferredName) : IQueryableFields
{
    public static string[] SearchFields => [nameof(Code), nameof(Status), nameof(ReferrerEmail), nameof(ReferredEmail)];
}
