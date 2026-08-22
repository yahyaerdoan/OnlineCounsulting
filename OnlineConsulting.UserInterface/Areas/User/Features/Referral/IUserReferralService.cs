using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.User.Features.Referral;

public interface IUserReferralService
{
    Task<string> GetMyCodeAsync(CancellationToken cancellationToken = default);
    Task<List<ReferralResponse>> GetMyReferralsAsync(CancellationToken cancellationToken = default);
    Task<AccountCreditSummaryResponse> GetMyCreditAsync(CancellationToken cancellationToken = default);
    Task<ApiEnvelope> RedeemAsync(string code, CancellationToken cancellationToken = default);
}

public record ReferralResponse(Guid Id, Guid ReferrerUserId, Guid ReferredUserId, string Code, string Status, decimal? RewardAmount, DateTimeOffset? RewardedAt);
public record AccountCreditResponse(Guid Id, decimal Amount, string Reason, string SourceType, Guid SourceId);
public record AccountCreditSummaryResponse(decimal Balance, List<AccountCreditResponse> Entries);
