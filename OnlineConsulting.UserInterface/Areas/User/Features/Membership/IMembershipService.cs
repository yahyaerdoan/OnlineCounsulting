using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.User.Features.Membership;

public interface IMembershipService
{
    Task<MembershipResponse?> GetMineAsync(CancellationToken cancellationToken = default);
    Task<PlanResponse?> GetPlanAsync(Guid planId, CancellationToken cancellationToken = default);
    Task<decimal> GetCreditBalanceAsync(CancellationToken cancellationToken = default);
    Task<ApiEnvelope<SubscribeResult>> SubscribeAsync(Guid planId, string paymentMethodId, decimal? creditToApply, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> CancelAsync(CancellationToken cancellationToken = default);
}

public record PlanResponse(Guid Id, string Name, string BillingCycle, decimal Price);
public record MembershipResponse(Guid Id, Guid UserId, Guid MembershipPlanId, string Status, DateTimeOffset StartDate, DateTimeOffset? RenewalDate);
public record SubscribeResult(Guid CustomerMembershipId, string? ClientSecret, decimal? AppliedCreditAmount);
