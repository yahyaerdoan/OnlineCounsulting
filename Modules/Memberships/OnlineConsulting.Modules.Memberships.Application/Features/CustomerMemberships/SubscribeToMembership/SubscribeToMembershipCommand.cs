using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Constants;
using OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.Abstractions;
using OnlineConsulting.Modules.Memberships.Domain;
using OnlineConsulting.SharedKernel.Payments;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.SubscribeToMembership;

/// <summary>UserId/Email are always resolved server-side from the authenticated caller, never trusted from the client (see CreateAppointmentCommand for the same convention). PaymentMethodId comes from the provider's client-side SDK (Stripe.js) - it must already be tokenized before this call, never a raw card number. Ignored by providers with no such concept (PayPal - see ISubscriptionGateway.CreateSubscriptionAsync). CreditToApplyAmount is clamped by the caller against the user's account credit balance only - this module has no knowledge of Referrals' AccountCredit ledger. The handler applies the final clamp against the plan's own price (which it already loads) and reports the actual amount used via SubscribeToMembershipResult.AppliedCreditAmount.</summary>
public record SubscribeToMembershipCommand(Guid UserId, string Email, Guid MembershipPlanId, string PaymentMethodId, decimal? CreditToApplyAmount = null)
    : IRequest<OperationDataResult<SubscribeToMembershipResult>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [];
}

public class SubscribeToMembershipHandler(
    ICustomerMembershipRepository membershipRepository,
    IMembershipPlanRepository planRepository,
    ISubscriptionGateway subscriptionGateway)
    : IRequestHandler<SubscribeToMembershipCommand, OperationDataResult<SubscribeToMembershipResult>>
{
    public async Task<OperationDataResult<SubscribeToMembershipResult>> Handle(SubscribeToMembershipCommand request, CancellationToken cancellationToken)
    {
        var plan = await planRepository.GetAsync(p => p.Id == request.MembershipPlanId, cancellationToken: cancellationToken);
        if (plan is null || plan.ProviderPriceId is null)
            return Result.NotFound<SubscribeToMembershipResult>(string.Format(CustomerMembershipMessages.MembershipPlanNotFoundFormat, request.MembershipPlanId));

        var hasActiveMembership = await membershipRepository.AnyAsync(m =>
            m.UserId == request.UserId &&
            m.Status != CustomerMembershipStatuses.Cancelled,
            cancellationToken: cancellationToken);

        if (hasActiveMembership)
            return Result.BadRequest<SubscribeToMembershipResult>(CustomerMembershipMessages.AlreadyHasActiveMembership);

        var appliedCreditAmount = request.CreditToApplyAmount is > 0
            ? Math.Min(request.CreditToApplyAmount.Value, plan.Price)
            : (decimal?)null;

        var customer = await subscriptionGateway.EnsureCustomerAsync(new EnsureCustomerRequest(request.UserId.ToString(), request.Email), cancellationToken);

        var membership = new CustomerMembership
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            MembershipPlanId = plan.Id,
            Status = CustomerMembershipStatuses.PendingPayment,
            StartDate = DateTimeOffset.UtcNow,
            ProviderCustomerId = customer.ProviderCustomerId,
        };

        var subscription = await subscriptionGateway.CreateSubscriptionAsync(
            new CreateSubscriptionRequest(customer.ProviderCustomerId, plan.ProviderPriceId, request.PaymentMethodId, membership.Id.ToString(), appliedCreditAmount),
            cancellationToken);

        membership.ProviderSubscriptionId = subscription.ProviderSubscriptionId;
        membership.RenewalDate = subscription.CurrentPeriodEnd;
        membership.Status = subscription.Status switch
        {
            PaymentStatuses.Succeeded => CustomerMembershipStatuses.Active,
            PaymentStatuses.Failed => CustomerMembershipStatuses.PastDue,
            _ => CustomerMembershipStatuses.PendingPayment,
        };

        await membershipRepository.AddAsync(membership);

        return Result.Created(new SubscribeToMembershipResult(membership.Id, subscription.ClientSecret, appliedCreditAmount), "Subscribed to membership plan successfully.");
    }
}
