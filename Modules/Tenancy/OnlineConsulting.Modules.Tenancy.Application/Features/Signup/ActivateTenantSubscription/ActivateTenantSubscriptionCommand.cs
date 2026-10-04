using MediatR;
using Microsoft.Extensions.Logging;
using OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Signup.Constants;
using OnlineConsulting.Modules.Tenancy.Application.Features.Signup.Contracts;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Payments;
using OnlineConsulting.SharedKernel.Tenancy;
using OnlineConsulting.SharedKernel.Transactions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Signup.ActivateTenantSubscription;

/// <summary>Bills the Pending items via ISubscriptionGateway; runs before CreateTenantAdminCommand (pay-first) and is reused by the authenticated retry endpoint. Not ITenancyTransactionRequest - it charges a real card mid-handler, so a rollback-on-throw would strand a captured charge.</summary>
public record ActivateTenantSubscriptionCommand(Guid TenantId, string PaymentMethodId)
    : IRequest<OperationDataResult<ActivateTenantSubscriptionResult>>, IBypassesTenantStatusCheck;

/// <summary>Idempotency keys include the payment method: a network retry of the same submission can't charge twice, while a new attempt after
/// a rollback gets a fresh key (Stripe keeps keys for 24h and would otherwise replay the cancelled subscription or reject the new parameters).
/// A declined first charge is a hard failure (Failed), not PastDue - PastDue means an already-paying tenant's renewal failed. A stale Failed status found once ProviderSubscriptionId is already set is treated as recoverable, since that id is only ever set after CreateSubscriptionAsync genuinely succeeded.</summary>
public class ActivateTenantSubscriptionHandler(ITenantRepository tenantRepository, ITenantSubscriptionRepository tenantSubscriptionRepository, ITenantSubscriptionItemRepository tenantSubscriptionItemRepository, IModuleOfferingRepository moduleOfferingRepository, ISubscriptionGateway subscriptionGateway,
    ILogger<ActivateTenantSubscriptionHandler> logger)
    : IRequestHandler<ActivateTenantSubscriptionCommand, OperationDataResult<ActivateTenantSubscriptionResult>>
{
    public async Task<OperationDataResult<ActivateTenantSubscriptionResult>> Handle(ActivateTenantSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var tenant = await tenantRepository.GetAsync(t => t.Id == request.TenantId, cancellationToken: cancellationToken);
        if (tenant is null)
        {
            return Result.NotFound<ActivateTenantSubscriptionResult>(SignupMessages.TenantNotFound);
        }

        if (tenant.IsHeldByStaff)
        {
            return Result.Conflict<ActivateTenantSubscriptionResult>(SignupMessages.TenantHeldByStaff);
        }

        var tenantSubscription = await tenantSubscriptionRepository.GetAsync(s => s.TenantId == tenant.Id, cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException($"Tenant {tenant.Id} has no TenantSubscription row.");

        var itemsPage = await tenantSubscriptionItemRepository
            .GetAllAsync(predicate: i => i.TenantSubscriptionId == tenantSubscription.Id, enableTracking: true, cancellationToken: cancellationToken);

        var pendingItems = itemsPage.Where(i => i.IsAwaitingBilling).ToList();

        var pendingModuleKeys = pendingItems.Select(i => i.ModuleKey).ToList();

        var offerings = await moduleOfferingRepository
            .GetAllAsync(predicate: m => pendingModuleKeys.Contains(m.Key), cancellationToken: cancellationToken);

        var offeringsByKey = offerings.ToDictionary(m => m.Key);

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
                var customer = await subscriptionGateway
                    .EnsureCustomerAsync(new EnsureCustomerRequest(tenantSubscription.Id.ToString(), tenant.PrimaryContactEmail), idempotencyKey: $"tenant-signup-customer:{tenant.Id}", cancellationToken: cancellationToken);

                providerCustomerId = customer.ProviderCustomerId;
                tenant.LinkProviderCustomer(providerCustomerId);

                _ = await tenantRepository.UpdateAsync(tenant, cancellationToken: cancellationToken);
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
                    idempotencyKey: $"tenant-signup-subscription:{tenantSubscription.Id}:{request.PaymentMethodId}",
                    cancellationToken: cancellationToken);

                if (subscription.Status == PaymentStatuses.Failed)
                {
                    tenant.FailSignup();
                    tenantSubscription.MarkFailed();
                    _ = await tenantRepository.UpdateAsync(tenant, cancellationToken: cancellationToken);
                    _ = await tenantSubscriptionRepository.UpdateAsync(tenantSubscription, cancellationToken: cancellationToken);
                    return Result.BadGateway<ActivateTenantSubscriptionResult>(SignupMessages.PaymentSetupFailed);
                }

                tenantSubscription.AttachProviderSubscription(subscription.ProviderSubscriptionId, subscription.CurrentPeriodEnd.UtcDateTime,
                    paid: subscription.Status == PaymentStatuses.Succeeded);
                _ = await tenantSubscriptionRepository.UpdateAsync(tenantSubscription, cancellationToken: cancellationToken);

                firstItem.Activate(subscription.FirstItemProviderId);
                _ = await tenantSubscriptionItemRepository.UpdateAsync(firstItem, cancellationToken: cancellationToken);

                clientSecret = subscription.ClientSecret;
                _ = pendingItems.Remove(firstItem);
            }
            else if (tenantSubscription.Status == TenantSubscriptionStatuses.Failed)
            {
                tenantSubscription.RecoverFromFailure();
                _ = await tenantSubscriptionRepository.UpdateAsync(tenantSubscription, cancellationToken: cancellationToken);
            }

            var activeProviderSubscriptionId = tenantSubscription.ProviderSubscriptionId
                ?? throw new InvalidOperationException($"TenantSubscription {tenantSubscription.Id} has no ProviderSubscriptionId after activation.");

            foreach (var item in pendingItems)
            {
                var providerSubscriptionItemId = item.ProviderSubscriptionItemId;
                if (providerSubscriptionItemId is null)
                {
                    var offering = offeringsByKey.TryGetValue(item.ModuleKey, out var o)
                        ? o
                        : throw new InvalidOperationException($"ModuleOffering {item.ModuleKey} was not found for pending TenantSubscriptionItem {item.Id}.");

                    var offeringPriceId = offering.ProviderPriceId
                        ?? throw new InvalidOperationException($"ModuleOffering {offering.Key} has no ProviderPriceId.");

                    providerSubscriptionItemId = await subscriptionGateway
                        .AddSubscriptionItemAsync(activeProviderSubscriptionId, offeringPriceId, idempotencyKey: $"tenant-signup-item:{activeProviderSubscriptionId}:{offering.Key}", cancellationToken: cancellationToken);
                }

                item.Activate(providerSubscriptionItemId);
                _ = await tenantSubscriptionItemRepository.UpdateAsync(item, cancellationToken: cancellationToken);
            }
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Tenant {TenantId}: activating the subscription failed.", tenant.Id);
            tenant.FailSignup();
            tenantSubscription.MarkFailed();
            _ = await tenantRepository.UpdateAsync(tenant, cancellationToken: cancellationToken);
            _ = await tenantSubscriptionRepository.UpdateAsync(tenantSubscription, cancellationToken: cancellationToken);
            return Result.BadGateway<ActivateTenantSubscriptionResult>(SignupMessages.PaymentSetupFailed);
        }

        tenant.CompleteSignup(tenantSubscription.Status);
        _ = await tenantRepository.UpdateAsync(tenant, cancellationToken: cancellationToken);

        return Result.Created(new ActivateTenantSubscriptionResult(tenant.Id, clientSecret), "Tenant subscription activated successfully.");
    }
}
