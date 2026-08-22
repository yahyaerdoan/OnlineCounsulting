using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.User.Features.Referral;

public class UserReferralService(IApiClient apiClient) : IUserReferralService
{
    private const string ReferralsPath = "/api/referrals";

    public async Task<string> GetMyCodeAsync(CancellationToken cancellationToken = default)
    {
        var result = await apiClient.PostAsync<string>($"{ReferralsPath}/my-code", null, cancellationToken);
        return result.ResultData ?? string.Empty;
    }

    public async Task<List<ReferralResponse>> GetMyReferralsAsync(CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<List<ReferralResponse>>($"{ReferralsPath}/mine", cancellationToken);
        return result.ResultData ?? [];
    }

    public async Task<AccountCreditSummaryResponse> GetMyCreditAsync(CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<AccountCreditSummaryResponse>($"{ReferralsPath}/my-credit", cancellationToken);
        return result.ResultData ?? new AccountCreditSummaryResponse(0, []);
    }

    public Task<ApiEnvelope> RedeemAsync(string code, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync($"{ReferralsPath}/redeem", new { Code = code }, cancellationToken);
}
