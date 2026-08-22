using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.User.Features.Membership;

public class MembershipService(IApiClient apiClient) : IMembershipService
{
    private const string MembershipsPath = "/api/memberships";
    private const string PlansPath = "/api/membership-plans";
    private const string CreditPath = "/api/referrals/my-credit";

    public async Task<MembershipResponse?> GetMineAsync(CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<MembershipResponse>($"{MembershipsPath}/mine", cancellationToken);
        return result.ResultData;
    }

    public async Task<PlanResponse?> GetPlanAsync(Guid planId, CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<PlanResponse>($"{PlansPath}/{planId}", cancellationToken);
        return result.ResultData;
    }

    public async Task<decimal> GetCreditBalanceAsync(CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<AccountCreditSummaryResponse>(CreditPath, cancellationToken);
        return result.ResultData?.Balance ?? 0;
    }

    public Task<ApiEnvelope<SubscribeResult>> SubscribeAsync(Guid planId, string paymentMethodId, decimal? creditToApply, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync<SubscribeResult>($"{MembershipsPath}/subscribe", new
        {
            MembershipPlanId = planId,
            PaymentMethodId = paymentMethodId,
            CreditToApplyAmount = creditToApply,
        }, cancellationToken);

    public Task<ApiEnvelope> CancelAsync(CancellationToken cancellationToken = default) =>
        apiClient.PostAsync($"{MembershipsPath}/cancel", null, cancellationToken);

    private record AccountCreditSummaryResponse(decimal Balance);
}
