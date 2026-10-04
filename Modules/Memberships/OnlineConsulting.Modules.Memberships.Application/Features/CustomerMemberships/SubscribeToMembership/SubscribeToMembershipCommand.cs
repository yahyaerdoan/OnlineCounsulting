using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Constants;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Contracts;
using OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes;
using OnlineConsulting.Modules.Memberships.Domain;
using OnlineConsulting.SharedKernel.Payments;
using OnlineConsulting.SharedKernel.Referrals;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.SubscribeToMembership;

/// <summary>PaymentMethodId must already be tokenized client-side (Stripe.js), never a raw card number; CreditToApplyAmount and PromoCode discounts combine into one gateway-side discount clamped to the plan's price.</summary>
public record SubscribeToMembershipCommand(Guid UserId,
                                           string Email,
                                           Guid MembershipPlanId,
                                           string PaymentMethodId,
                                           decimal? CreditToApplyAmount = null,
                                           string? PromoCode = null)
    : IRequest<OperationDataResult<SubscribeToMembershipResult>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [];
}

/// <summary>Deliberately not ITransactionAddRequest - a real card charge happens partway through, so each repository call saves immediately instead of rolling back and losing the charge's trace. Account credit is reserved before the charge and reversed if the charge fails.</summary>
public class SubscribeToMembershipHandler(ICustomerMembershipRepository membershipRepository,
                                          IMembershipPlanRepository planRepository,
                                          IPromoCodeRepository promoCodeRepository,
                                          ISubscriptionGateway subscriptionGateway,
                                          IAccountCreditLedger creditLedger,
                                          MembershipReceiptSender receiptSender,
                                          IMembershipNotifier notifier)
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

        var now = DateTimeOffset.UtcNow;
        PromoCode? promo = null;
        decimal promoDiscountAmount = 0;

        if (request.PromoCode is not null)
        {
            var normalizedCode = PromoCode.Normalize(request.PromoCode);

            promo = await promoCodeRepository.GetAsync(p => p.Code == normalizedCode, cancellationToken: cancellationToken);

            var alreadyRedeemed = promo is not null && await membershipRepository.AnyAsync(m => m.UserId == request.UserId && m.PromoCodeId == promo.Id, cancellationToken: cancellationToken);

            var (isValid, error, amount) = PromoCodeEvaluator.Evaluate(promo, plan, alreadyRedeemed, now);

            if (!isValid)
            {
                return Result.UnprocessableContent<SubscribeToMembershipResult>(error ?? CustomerMembershipMessages.PaymentSetupFailed);
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
                    return Result.Conflict<SubscribeToMembershipResult>(CustomerMembershipMessages.PreviousAttemptNeedsSupport);
                }

                stalePlanMembership.SwitchPendingPlan(request.MembershipPlanId);

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
                return Result.Conflict<SubscribeToMembershipResult>(CustomerMembershipMessages.AlreadyHasActiveMembership);
            }
        }

        if (membership is null)
        {
            membership = CustomerMembership.Start(request.UserId, plan.Id, DateTimeOffset.UtcNow);

            _ = await membershipRepository.AddAsync(membership);
        }

        string? clientSecret;
        decimal? appliedCreditAmount;
        var creditReserved = false;
        var subscriptionCreated = false;

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
                membership.LinkProviderCustomer(providerCustomerId);

                _ = await membershipRepository.UpdateAsync(membership);
            }

            if (membership.ProviderSubscriptionId is null)
            {
                var creditToApply = await ResolveCreditToApplyAsync(request, membership.Id, plan.Price - promoDiscountAmount, cancellationToken);
                if (!await creditLedger.TryDebitAsync(request.UserId, creditToApply, CustomerMembershipMessages.CreditAppliedReason, AccountCreditSourceTypes.MembershipDiscount, membership.Id, cancellationToken))
                {
                    return Result.Conflict<SubscribeToMembershipResult>(CustomerMembershipMessages.InsufficientCredit);
                }

                creditReserved = creditToApply > 0;
                var totalDiscountAmount = Math.Min(creditToApply + promoDiscountAmount, plan.Price);

                var subscription = await subscriptionGateway
                    .CreateSubscriptionAsync(new CreateSubscriptionRequest(providerCustomerId, plan.ProviderPriceId, request.PaymentMethodId, membership.Id.ToString(),
                    totalDiscountAmount is > 0 ? totalDiscountAmount : null, plan.TrialDays), idempotencyKey: $"membership-signup-subscription:{membership.Id}:{request.PaymentMethodId}", cancellationToken: cancellationToken);

                creditReserved = false;
                subscriptionCreated = true;
                membership.AttachSubscription(subscription.ProviderSubscriptionId, subscription.CurrentPeriodEnd, plan.TrialDays is > 0 ? DateTimeOffset.UtcNow.AddDays(plan.TrialDays.Value) : null, promo?.Id);

                if (subscription.Status == PaymentStatuses.Succeeded)
                {
                    membership.Activate();
                }
                else if (subscription.Status == PaymentStatuses.Failed)
                {
                    membership.MarkPaymentFailed(DateTimeOffset.UtcNow);
                }

                if (promo is not null)
                {
                    promo.Redeem(plan.Id, now);

                    _ = await promoCodeRepository.UpdateAsync(promo);
                }

                _ = await membershipRepository.UpdateAsync(membership);

                clientSecret = subscription.ClientSecret;
                appliedCreditAmount = creditToApply > 0 ? creditToApply : null;
            }
            else
            {
                if (membership.Status == CustomerMembershipStatuses.Failed)
                {
                    membership.Activate();

                    _ = await membershipRepository.UpdateAsync(membership);
                }

                clientSecret = null;
                var debited = await creditLedger.GetDebitedAsync(request.UserId, AccountCreditSourceTypes.MembershipDiscount, membership.Id, cancellationToken);
                appliedCreditAmount = debited > 0 ? debited : null;
            }
        }
        catch (Exception)
        {
            membership.MarkSignupFailed();

            _ = await membershipRepository.UpdateAsync(membership);

            if (creditReserved)
            {
                await creditLedger.ReverseAsync(request.UserId, CustomerMembershipMessages.CreditReturnedReason, AccountCreditSourceTypes.MembershipDiscount, membership.Id, cancellationToken);
            }

            return Result.BadGateway<SubscribeToMembershipResult>(CustomerMembershipMessages.PaymentSetupFailed);
        }

        if (subscriptionCreated && membership.Status == CustomerMembershipStatuses.Active)
        {
            await receiptSender.SendLatestAsync(membership, cancellationToken);

            await notifier.StartedAsync(membership, cancellationToken);
        }

        return Result.Created(new SubscribeToMembershipResult(membership.Id, clientSecret, appliedCreditAmount, promo is not null ? promoDiscountAmount : null), "Subscribed to membership plan successfully.");
    }

    private async Task<decimal> ResolveCreditToApplyAsync(SubscribeToMembershipCommand request, Guid membershipId, decimal payableAmount, CancellationToken cancellationToken)
    {
        if (request.CreditToApplyAmount is not > 0 || payableAmount <= 0)
        {
            return 0;
        }

        var available = await creditLedger.GetBalanceAsync(request.UserId, cancellationToken)
            + await creditLedger.GetDebitedAsync(request.UserId, AccountCreditSourceTypes.MembershipDiscount, membershipId, cancellationToken);

        return Math.Max(0, Math.Min(request.CreditToApplyAmount.Value, Math.Min(payableAmount, available)));
    }
}
