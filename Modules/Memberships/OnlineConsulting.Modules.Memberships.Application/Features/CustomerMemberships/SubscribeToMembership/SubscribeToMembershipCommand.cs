using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Constants;
using OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.Common;
using OnlineConsulting.Modules.Memberships.Domain;
using OnlineConsulting.SharedKernel.Payments;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.SubscribeToMembership;

/// <summary>PaymentMethodId must already be tokenized client-side (Stripe.js), never a raw card number; CreditToApplyAmount and PromoCode discounts combine into one gateway-side discount clamped to the plan's price.</summary>
public record SubscribeToMembershipCommand(Guid UserId, string Email, Guid MembershipPlanId, string PaymentMethodId, decimal? CreditToApplyAmount = null, string? PromoCode = null)
    : IRequest<OperationDataResult<SubscribeToMembershipResult>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [];
}

/// <summary>Deliberately not ITransactionAddRequest - a real card charge happens partway through, so each repository call saves immediately instead of rolling back and losing the charge's trace.</summary>
public class SubscribeToMembershipHandler(ICustomerMembershipRepository membershipRepository, IMembershipPlanRepository planRepository, IPromoCodeRepository promoCodeRepository, ISubscriptionGateway subscriptionGateway)
    : IRequestHandler<SubscribeToMembershipCommand, OperationDataResult<SubscribeToMembershipResult>>
{
    /// <summary>Creates or resumes a pending subscription attempt; a Failed status is only stale here since ProviderSubscriptionId is set only after CreateSubscriptionAsync genuinely succeeds.</summary>
    public async Task<OperationDataResult<SubscribeToMembershipResult>> Handle(SubscribeToMembershipCommand request, CancellationToken cancellationToken)
    {
        var plan = await planRepository.GetAsync(p => p.Id == request.MembershipPlanId, cancellationToken: cancellationToken);
        if (plan is null || plan.ProviderPriceId is null || !plan.IsActive)
        {
            return Result.NotFound<SubscribeToMembershipResult>(string.Format(CustomerMembershipMessages.MembershipPlanNotFoundFormat, request.MembershipPlanId));
        }

        PromoCode? promo = null;
        decimal promoDiscountAmount = 0;

        if (request.PromoCode is not null)
        {
            var normalizedCode = request.PromoCode.Trim().ToUpperInvariant();
            promo = await promoCodeRepository.GetAsync(p => p.Code == normalizedCode, cancellationToken: cancellationToken);

            var alreadyRedeemed = promo is not null && await membershipRepository.AnyAsync(m => m.UserId == request.UserId && m.PromoCodeId == promo.Id, cancellationToken: cancellationToken);

            var (isValid, error, amount) = PromoCodeEvaluator.Evaluate(promo, plan, alreadyRedeemed);
            if (!isValid)
            {
                return Result.BadRequest<SubscribeToMembershipResult>(error!);
            }

            promoDiscountAmount = amount;
        }

        var membership = await membershipRepository.GetAsync(m =>
        m.UserId == request.UserId && m.MembershipPlanId == request.MembershipPlanId && (m.Status == CustomerMembershipStatuses.PendingPayment || m.Status == CustomerMembershipStatuses.Failed), cancellationToken: cancellationToken);

        if (membership is null)
        {
            var stalePlanMembership = await membershipRepository.GetAsync(m =>
                m.UserId == request.UserId && m.MembershipPlanId != request.MembershipPlanId && (m.Status == CustomerMembershipStatuses.PendingPayment || m.Status == CustomerMembershipStatuses.Failed), cancellationToken: cancellationToken);

            if (stalePlanMembership is not null)
            {
                if (stalePlanMembership.ProviderSubscriptionId is not null)
                {
                    return Result.BadRequest<SubscribeToMembershipResult>(CustomerMembershipMessages.PreviousAttemptNeedsSupport);
                }

                stalePlanMembership.MembershipPlanId = request.MembershipPlanId;

                _ = await membershipRepository.UpdateAsync(stalePlanMembership);

                membership = stalePlanMembership;
            }
        }

        if (membership is null)
        {
            var hasActiveMembership = await membershipRepository.AnyAsync(m =>
                m.UserId == request.UserId && m.Status != CustomerMembershipStatuses.Cancelled, cancellationToken: cancellationToken);

            if (hasActiveMembership)
            {
                return Result.BadRequest<SubscribeToMembershipResult>(CustomerMembershipMessages.AlreadyHasActiveMembership);
            }
        }

        var appliedCreditAmount = request.CreditToApplyAmount is > 0
            ? Math.Min(request.CreditToApplyAmount.Value, plan.Price)
            : (decimal?)null;

        var totalDiscountAmount = Math.Min((appliedCreditAmount ?? 0) + promoDiscountAmount, plan.Price);

        if (membership is null)
        {
            membership = new CustomerMembership
            {
                UserId = request.UserId,
                MembershipPlanId = plan.Id,
                Status = CustomerMembershipStatuses.PendingPayment,
                StartDate = DateTimeOffset.UtcNow,
            };

            _ = await membershipRepository.AddAsync(membership);
        }

        string? clientSecret;
        try
        {
            string providerCustomerId;
            if (membership.ProviderCustomerId is not null)
            {
                providerCustomerId = membership.ProviderCustomerId;
            }
            else
            {
                var customer = await subscriptionGateway
                    .EnsureCustomerAsync(new EnsureCustomerRequest(request.UserId.ToString(), request.Email), idempotencyKey: $"membership-signup-customer:{membership.Id}", cancellationToken: cancellationToken);

                providerCustomerId = customer.ProviderCustomerId;
                membership.ProviderCustomerId = providerCustomerId;

                _ = await membershipRepository.UpdateAsync(membership);
            }

            if (membership.ProviderSubscriptionId is null)
            {
                var subscription = await subscriptionGateway
                    .CreateSubscriptionAsync(new CreateSubscriptionRequest(providerCustomerId, plan.ProviderPriceId, request.PaymentMethodId, membership.Id.ToString(),
                    totalDiscountAmount is > 0 ? totalDiscountAmount : null, plan.TrialDays), idempotencyKey: $"membership-signup-subscription:{membership.Id}", cancellationToken: cancellationToken);

                membership.ProviderSubscriptionId = subscription.ProviderSubscriptionId;
                membership.RenewalDate = subscription.CurrentPeriodEnd;
                membership.TrialEndDate = plan.TrialDays is > 0 ? DateTimeOffset.UtcNow.AddDays(plan.TrialDays.Value) : null;

                membership.Status = subscription.Status switch
                {
                    PaymentStatuses.Succeeded => CustomerMembershipStatuses.Active,
                    PaymentStatuses.Failed => CustomerMembershipStatuses.PastDue,
                    _ => CustomerMembershipStatuses.PendingPayment,
                };

                if (promo is not null)
                {
                    membership.PromoCodeId = promo.Id;
                    promo.RedemptionCount++;

                    _ = await promoCodeRepository.UpdateAsync(promo);
                }

                _ = await membershipRepository.UpdateAsync(membership);

                clientSecret = subscription.ClientSecret;
            }
            else
            {
                if (membership.Status == CustomerMembershipStatuses.Failed)
                {
                    membership.Status = CustomerMembershipStatuses.Active;

                    _ = await membershipRepository.UpdateAsync(membership);
                }

                clientSecret = null;
            }
        }
        catch (Exception)
        {
            membership.Status = CustomerMembershipStatuses.Failed;

            _ = await membershipRepository.UpdateAsync(membership);

            return Result.BadRequest<SubscribeToMembershipResult>(CustomerMembershipMessages.PaymentSetupFailed);
        }

        return Result.Created(new SubscribeToMembershipResult(membership.Id, clientSecret, appliedCreditAmount, promo is not null ? promoDiscountAmount : null), "Subscribed to membership plan successfully.");
    }
}
