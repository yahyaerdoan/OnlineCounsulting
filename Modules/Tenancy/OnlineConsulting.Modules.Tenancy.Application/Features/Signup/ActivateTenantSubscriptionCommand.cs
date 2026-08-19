using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Signup.Constants;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Payments;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Signup;

/// <summary>Second half of self-service tenant signup - bills the Pending TenantSubscriptionItem rows that ReserveTenantCommand already recorded, via ISubscriptionGateway (EnsureCustomerAsync/CreateSubscriptionAsync for the first item, AddSubscriptionItemAsync for the rest). Deliberately plain IRequest, not ISecureAddRequest: OnlineConsulting.Api/Features/Tenancy/SignUp.cs sends this right after CreateTenantAdminCommand succeeds, still inside the anonymous public signup endpoint (no JWT/tenant claim exists yet), so it cannot depend on TenantOwnershipGuard/ITenantProvider the way AddModuleCommand/RemoveModuleCommand do. A tenant stuck at PendingPayment/Failed after this fails (Stripe error) already has its Tenant/User rows, so it can retry billing alone via the separate authenticated POST /api/tenancy/{tenantId}/activate endpoint - that endpoint applies the TenantOwnershipGuard check itself before sending this same command.
/// Resumable/retryable on its own for the same TenantId, exactly like the old combined SignUpTenantCommand was: each TenantSubscriptionItem is loaded by its persisted Status (Pending/Failed still need billing, Active already succeeded), ProviderSubscriptionId/ProviderSubscriptionItemId already set are skipped rather than re-billed, and a stale Failed TenantSubscription.Status left over from a response that failed to parse after a genuinely-successful Stripe call is corrected back to Active on resume.
/// Deliberately NOT ITransactionAddRequest, same reasoning as the old SignUpTenantCommand: the handler charges a real card via ISubscriptionGateway partway through, and a DB transaction that only commits after the whole handler returns would roll back local status/id updates if a later step throws - leaving a real Stripe charge with zero trace in this database. Every repository call below is its own immediately-saved write.</summary>
public record ActivateTenantSubscriptionCommand(Guid TenantId, string PaymentMethodId) : IRequest<OperationDataResult<ActivateTenantSubscriptionResult>>;

/// <summary>ClientSecret mirrors SubscribeToMembershipResult - null for Stripe, a PayPal approval URL when PayPal is active. Null on a resume call where the base subscription was already created in a prior attempt.</summary>
public record ActivateTenantSubscriptionResult(Guid TenantId, string? ClientSecret);

public class ActivateTenantSubscriptionHandler(ITenantRepository tenantRepository, ITenantSubscriptionRepository tenantSubscriptionRepository, ITenantSubscriptionItemRepository tenantSubscriptionItemRepository, IModuleOfferingRepository moduleOfferingRepository, ISubscriptionGateway subscriptionGateway)
    : IRequestHandler<ActivateTenantSubscriptionCommand, OperationDataResult<ActivateTenantSubscriptionResult>>
{
    public async Task<OperationDataResult<ActivateTenantSubscriptionResult>> Handle(ActivateTenantSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var tenant = await tenantRepository.GetAsync(t => t.Id == request.TenantId, cancellationToken: cancellationToken);
        if (tenant is null)
            return Result.NotFound<ActivateTenantSubscriptionResult>(SignupMessages.TenantNotFound);

        var tenantSubscription = await tenantSubscriptionRepository.GetAsync(s => s.TenantId == tenant.Id, cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException($"Tenant {tenant.Id} has no TenantSubscription row.");

        var itemsPage = await tenantSubscriptionItemRepository
            .GetListAsync(predicate: i => i.TenantSubscriptionId == tenantSubscription.Id, size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);

        var pendingItems = itemsPage.Items
            .Where(i => i.Status == TenantSubscriptionItemStatuses.Pending || i.Status == TenantSubscriptionItemStatuses.Failed)
            .ToList();

        var pendingModuleKeys = pendingItems.Select(i => i.ModuleKey).ToList();

        var offerings = await moduleOfferingRepository
            .GetListAsync(predicate: m => pendingModuleKeys.Contains(m.Key), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);

        var offeringsByKey = offerings.Items.ToDictionary(m => m.Key);

        string? clientSecret = null;
        try
        {
            string providerCustomerId;
            if (tenant.ProviderCustomerId is not null)
            {
                providerCustomerId = tenant.ProviderCustomerId;
            }
            else
            {
                var customer = await subscriptionGateway.EnsureCustomerAsync(
                    new EnsureCustomerRequest(tenantSubscription.Id.ToString(), tenant.PrimaryContactEmail),
                    idempotencyKey: $"tenant-signup-customer:{tenant.Id}",
                    cancellationToken: cancellationToken);
                providerCustomerId = customer.ProviderCustomerId;
                tenant.ProviderCustomerId = providerCustomerId;
                await tenantRepository.UpdateAsync(tenant);
            }

            if (tenantSubscription.ProviderSubscriptionId is null)
            {
                var firstItem = pendingItems.FirstOrDefault()
                    ?? throw new InvalidOperationException($"TenantSubscription {tenantSubscription.Id} has no ProviderSubscriptionId yet, but has no pending TenantSubscriptionItem to create it from.");
                var firstOffering = offeringsByKey.TryGetValue(firstItem.ModuleKey, out var offering)
                    ? offering
                    : throw new InvalidOperationException($"ModuleOffering {firstItem.ModuleKey} was not found for pending TenantSubscriptionItem {firstItem.Id}.");
                var firstOfferingPriceId = firstOffering.ProviderPriceId
                    ?? throw new InvalidOperationException($"ModuleOffering {firstOffering.Key} has no ProviderPriceId.");

                var subscription = await subscriptionGateway.CreateSubscriptionAsync(
                    new CreateSubscriptionRequest(providerCustomerId, firstOfferingPriceId, request.PaymentMethodId, tenantSubscription.Id.ToString()),
                    idempotencyKey: $"tenant-signup-subscription:{tenantSubscription.Id}",
                    cancellationToken: cancellationToken);

                tenantSubscription.ProviderSubscriptionId = subscription.ProviderSubscriptionId;
                tenantSubscription.RenewalDate = subscription.CurrentPeriodEnd.UtcDateTime;
                tenantSubscription.Status = subscription.Status switch
                {
                    PaymentStatuses.Succeeded => TenantSubscriptionStatuses.Active,
                    PaymentStatuses.Failed => TenantSubscriptionStatuses.PastDue,
                    _ => TenantSubscriptionStatuses.PendingPayment,
                };
                await tenantSubscriptionRepository.UpdateAsync(tenantSubscription);

                firstItem.ProviderSubscriptionItemId = subscription.FirstItemProviderId;
                firstItem.Status = TenantSubscriptionItemStatuses.Active;
                await tenantSubscriptionItemRepository.UpdateAsync(firstItem);

                clientSecret = subscription.ClientSecret;
                pendingItems.Remove(firstItem);
            }
            else if (tenantSubscription.Status == TenantSubscriptionStatuses.Failed)
            {
                // ProviderSubscriptionId only gets set once CreateSubscriptionAsync genuinely succeeds, so a Failed status here is stale, not a real failure.
                tenantSubscription.Status = TenantSubscriptionStatuses.Active;
                await tenantSubscriptionRepository.UpdateAsync(tenantSubscription);
            }

            foreach (var item in pendingItems)
            {
                if (item.ProviderSubscriptionItemId is null)
                {
                    var offering = offeringsByKey.TryGetValue(item.ModuleKey, out var o)
                        ? o
                        : throw new InvalidOperationException($"ModuleOffering {item.ModuleKey} was not found for pending TenantSubscriptionItem {item.Id}.");

                    var offeringPriceId = offering.ProviderPriceId
                        ?? throw new InvalidOperationException($"ModuleOffering {offering.Key} has no ProviderPriceId.");

                    item.ProviderSubscriptionItemId = await subscriptionGateway
                        .AddSubscriptionItemAsync(tenantSubscription.ProviderSubscriptionId, offeringPriceId, idempotencyKey: $"tenant-signup-item:{tenantSubscription.Id}:{offering.Key}", cancellationToken: cancellationToken);
                }

                item.Status = TenantSubscriptionItemStatuses.Active;
                await tenantSubscriptionItemRepository.UpdateAsync(item);
            }
        }
        catch (Exception)
        {
            tenant.Status = TenantStatuses.Failed;
            tenantSubscription.Status = TenantSubscriptionStatuses.Failed;
            await tenantRepository.UpdateAsync(tenant);
            await tenantSubscriptionRepository.UpdateAsync(tenantSubscription);
            return Result.BadRequest<ActivateTenantSubscriptionResult>(SignupMessages.PaymentSetupFailed);
        }

        tenant.Status = tenantSubscription.Status switch
        {
            TenantSubscriptionStatuses.Active => TenantStatuses.Active,
            TenantSubscriptionStatuses.PastDue => TenantStatuses.PastDue,
            _ => TenantStatuses.PendingPayment,
        };
        await tenantRepository.UpdateAsync(tenant);

        return Result.Created(new ActivateTenantSubscriptionResult(tenant.Id, clientSecret), "Tenant subscription activated successfully.");
    }
}
