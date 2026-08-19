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

/// <summary>UserId/Email are always resolved server-side from the authenticated caller, never trusted from the client (see CreateAppointmentCommand for the same convention). PaymentMethodId comes from the provider's client-side SDK (Stripe.js) - it must already be tokenized before this call, never a raw card number. Ignored by providers with no such concept (PayPal - see ISubscriptionGateway.CreateSubscriptionAsync). CreditToApplyAmount is clamped by the caller against the user's account credit balance only - this module has no knowledge of Referrals' AccountCredit ledger. The handler applies the final clamp against the plan's own price (which it already loads) and reports the actual amount used via SubscribeToMembershipResult.AppliedCreditAmount.
/// The Pending/Failed dedup lookup below is check-then-act with no locking (same residual race as SignUp.cs's email pre-check, see ARCHITECTURE_MIGRATION.md) - two near-simultaneous requests for the same user+plan can each miss the check and create their own row/subscription. No reusable distributed-lock abstraction was found in this repo's referenced packages (Core.ApplicationLayer's CacheAddingBehavior has an internal HandleWithDistributedLockAsync, but it's private plumbing scoped to cache-key population, not an injectable general-purpose lock), so this is accepted as a low-probability residual risk rather than closed with app-level locking.</summary>
public record SubscribeToMembershipCommand(Guid UserId, string Email, Guid MembershipPlanId, string PaymentMethodId, decimal? CreditToApplyAmount = null)
    : IRequest<OperationDataResult<SubscribeToMembershipResult>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [];
}

/// <summary>Deliberately NOT ITransactionAddRequest (this command doesn't opt into one already): the handler charges a real card via ISubscriptionGateway partway through, and a DB transaction that only commits after the whole handler returns would roll back the local CustomerMembership row if a later step throws - leaving a real Stripe charge with zero trace in this database. Every repository call below is its own immediately-saved write (see EfRepositoryBase.AddAsync/UpdateAsync), so each step is durable the instant it happens, independent of anything that runs after it.</summary>
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

        var membership = await membershipRepository.GetAsync(m =>
            m.UserId == request.UserId &&
            m.MembershipPlanId == request.MembershipPlanId &&
            (m.Status == CustomerMembershipStatuses.PendingPayment || m.Status == CustomerMembershipStatuses.Failed),
            cancellationToken: cancellationToken);

        if (membership is null)
        {
            var stalePlanMembership = await membershipRepository.GetAsync(m =>
                m.UserId == request.UserId &&
                m.MembershipPlanId != request.MembershipPlanId &&
                (m.Status == CustomerMembershipStatuses.PendingPayment || m.Status == CustomerMembershipStatuses.Failed),
                cancellationToken: cancellationToken);

            if (stalePlanMembership is not null)
            {
                if (stalePlanMembership.ProviderSubscriptionId is not null)
                    return Result.BadRequest<SubscribeToMembershipResult>(CustomerMembershipMessages.PreviousAttemptNeedsSupport);

                stalePlanMembership.MembershipPlanId = request.MembershipPlanId;
                await membershipRepository.UpdateAsync(stalePlanMembership);
                membership = stalePlanMembership;
            }
        }

        if (membership is null)
        {
            var hasActiveMembership = await membershipRepository.AnyAsync(m =>
                m.UserId == request.UserId &&
                m.Status != CustomerMembershipStatuses.Cancelled,
                cancellationToken: cancellationToken);

            if (hasActiveMembership)
                return Result.BadRequest<SubscribeToMembershipResult>(CustomerMembershipMessages.AlreadyHasActiveMembership);
        }

        var appliedCreditAmount = request.CreditToApplyAmount is > 0
            ? Math.Min(request.CreditToApplyAmount.Value, plan.Price)
            : (decimal?)null;

        if (membership is null)
        {
            membership = new CustomerMembership
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                MembershipPlanId = plan.Id,
                Status = CustomerMembershipStatuses.PendingPayment,
                StartDate = DateTimeOffset.UtcNow,
            };

            await membershipRepository.AddAsync(membership);
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
                var customer = await subscriptionGateway.EnsureCustomerAsync(
                    new EnsureCustomerRequest(request.UserId.ToString(), request.Email),
                    idempotencyKey: $"membership-signup-customer:{membership.Id}",
                    cancellationToken: cancellationToken);
                providerCustomerId = customer.ProviderCustomerId;
                membership.ProviderCustomerId = providerCustomerId;
                await membershipRepository.UpdateAsync(membership);
            }

            if (membership.ProviderSubscriptionId is null)
            {
                var subscription = await subscriptionGateway.CreateSubscriptionAsync(
                    new CreateSubscriptionRequest(providerCustomerId, plan.ProviderPriceId, request.PaymentMethodId, membership.Id.ToString(), appliedCreditAmount),
                    idempotencyKey: $"membership-signup-subscription:{membership.Id}",
                    cancellationToken: cancellationToken);

                membership.ProviderSubscriptionId = subscription.ProviderSubscriptionId;
                membership.RenewalDate = subscription.CurrentPeriodEnd;
                membership.Status = subscription.Status switch
                {
                    PaymentStatuses.Succeeded => CustomerMembershipStatuses.Active,
                    PaymentStatuses.Failed => CustomerMembershipStatuses.PastDue,
                    _ => CustomerMembershipStatuses.PendingPayment,
                };
                await membershipRepository.UpdateAsync(membership);

                clientSecret = subscription.ClientSecret;
            }
            else
            {
                if (membership.Status == CustomerMembershipStatuses.Failed)
                {
                    // ProviderSubscriptionId only gets set once CreateSubscriptionAsync genuinely succeeds, so a Failed status here is stale, not a real failure.
                    membership.Status = CustomerMembershipStatuses.Active;
                    await membershipRepository.UpdateAsync(membership);
                }

                clientSecret = null;
            }
        }
        catch (Exception)
        {
            membership.Status = CustomerMembershipStatuses.Failed;
            await membershipRepository.UpdateAsync(membership);
            return Result.BadRequest<SubscribeToMembershipResult>(CustomerMembershipMessages.PaymentSetupFailed);
        }

        return Result.Created(new SubscribeToMembershipResult(membership.Id, clientSecret, appliedCreditAmount), "Subscribed to membership plan successfully.");
    }
}
