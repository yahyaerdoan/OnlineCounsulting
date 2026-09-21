using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.User.Features.Referral;

public interface IUserReferralService
{
    /// <summary>Gets the current user's referral code, creating it on first call.</summary>
    Task<string> GetMyCodeAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets referrals made by the current user.</summary>
    Task<List<ReferralResponse>> GetMyReferralsAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets the current user's account credit balance and entries.</summary>
    Task<AccountCreditSummaryResponse> GetMyCreditAsync(CancellationToken cancellationToken = default);

    /// <summary>Redeems a referral code for the current user.</summary>
    Task<ApiEnvelope> RedeemAsync(string code, CancellationToken cancellationToken = default);
}

public record ReferralResponse(Guid Id, Guid ReferrerUserId, Guid ReferredUserId, string Code, string Status, decimal? RewardAmount, DateTimeOffset? RewardedAt);
public record AccountCreditResponse(Guid Id, decimal Amount, string Reason, string SourceType, Guid SourceId);
public record AccountCreditSummaryResponse(decimal Balance, List<AccountCreditResponse> Entries);
