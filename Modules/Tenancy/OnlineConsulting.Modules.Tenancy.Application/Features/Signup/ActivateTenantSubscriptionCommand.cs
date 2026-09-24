using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Signup.Constants;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Payments;
using OnlineConsulting.SharedKernel.Persistence;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Signup;

/// <summary>Bills the Pending items via ISubscriptionGateway; runs before CreateTenantAdminCommand (pay-first) and is reused by the authenticated retry endpoint. Not ITransactionAddRequest - it charges a real card mid-handler, so a rollback-on-throw would strand a captured charge.</summary>
public record ActivateTenantSubscriptionCommand(Guid TenantId, string PaymentMethodId)
    : IRequest<OperationDataResult<ActivateTenantSubscriptionResult>>, IBypassesTenantStatusCheck;

/// <summary>ClientSecret mirrors SubscribeToMembershipResult - null for Stripe, a PayPal approval URL when PayPal is active. Null on a resume call where the base subscription was already created in a prior attempt.</summary>
public record ActivateTenantSubscriptionResult(Guid TenantId, string? ClientSecret);

/// <summary>A declined first charge is a hard failure (Failed), not PastDue - PastDue means an already-paying tenant's renewal failed. A stale Failed status found once ProviderSubscriptionId is already set is treated as recoverable, since that id is only ever set after CreateSubscriptionAsync genuinely succeeded.</summary>
public class ActivateTenantSubscriptionHandler(ITenantRepository tenantRepository, ITenantSubscriptionRepository tenantSubscriptionRepository, ITenantSubscriptionItemRepository tenantSubscriptionItemRepository, IModuleOfferingRepository moduleOfferingRepository, ISubscriptionGateway subscriptionGateway)
    : IRequestHandler<ActivateTenantSubscriptionCommand, OperationDataResult<ActivateTenantSubscriptionResult>>
{
    public async Task<OperationDataResult<ActivateTenantSubscriptionResult>> Handle(ActivateTenantSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var tenant = await tenantRepository.GetAsync(t => t.Id == request.TenantId, cancellationToken: cancellationToken);
        if (tenant is null)
        {
            return Result.NotFound<ActivateTenantSubscriptionResult>(SignupMessages.TenantNotFound);
        }

        var tenantSubscription = await tenantSubscriptionRepository.GetAsync(s => s.TenantId == tenant.Id, cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException($"Tenant {tenant.Id} has no TenantSubscription row.");

        var itemsPage = await tenantSubscriptionItemRepository
            .GetListAsync(predicate: i => i.TenantSubscriptionId == tenantSubscription.Id, orderBy: q => q.OrderBy(i => i.Id), size: RepositoryQuerySize.Unbounded, enableTracking: true, cancellationToken: cancellationToken);

        var pendingItems = itemsPage.Items
            .Where(i => i.Status is TenantSubscriptionItemStatuses.Pending or TenantSubscriptionItemStatuses.Failed)
            .ToList();

        var pendingModuleKeys = pendingItems.Select(i => i.ModuleKey).ToList();

        var offerings = await moduleOfferingRepository
            .GetListAsync(predicate: m => pendingModuleKeys.Contains(m.Key), orderBy: q => q.OrderBy(m => m.Id), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);

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
                var customer = await subscriptionGateway
                    .EnsureCustomerAsync(new EnsureCustomerRequest(tenantSubscription.Id.ToString(), tenant.PrimaryContactEmail), idempotencyKey: $"tenant-signup-customer:{tenant.Id}", cancellationToken: cancellationToken);

                providerCustomerId = customer.ProviderCustomerId;
                tenant.ProviderCustomerId = providerCustomerId;

                _ = await tenantRepository.UpdateAsync(tenant);
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

                if (subscription.Status == PaymentStatuses.Failed)
                {
                    tenant.Status = TenantStatuses.Failed;
                    tenantSubscription.Status = TenantSubscriptionStatuses.Failed;
                    _ = await tenantRepository.UpdateAsync(tenant);
                    _ = await tenantSubscriptionRepository.UpdateAsync(tenantSubscription);
                    return Result.BadRequest<ActivateTenantSubscriptionResult>(SignupMessages.PaymentSetupFailed);
                }

                tenantSubscription.ProviderSubscriptionId = subscription.ProviderSubscriptionId;
                tenantSubscription.RenewalDate = subscription.CurrentPeriodEnd.UtcDateTime;
                tenantSubscription.Status = subscription.Status == PaymentStatuses.Succeeded
                    ? TenantSubscriptionStatuses.Active
                    : TenantSubscriptionStatuses.PendingPayment;
                _ = await tenantSubscriptionRepository.UpdateAsync(tenantSubscription);

                firstItem.ProviderSubscriptionItemId = subscription.FirstItemProviderId;
                firstItem.Status = TenantSubscriptionItemStatuses.Active;
                _ = await tenantSubscriptionItemRepository.UpdateAsync(firstItem);

                clientSecret = subscription.ClientSecret;
                _ = pendingItems.Remove(firstItem);
            }
            else if (tenantSubscription.Status == TenantSubscriptionStatuses.Failed)
            {
                tenantSubscription.Status = TenantSubscriptionStatuses.Active;
                _ = await tenantSubscriptionRepository.UpdateAsync(tenantSubscription);
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
                _ = await tenantSubscriptionItemRepository.UpdateAsync(item);
            }
        }
        catch (Exception)
        {
            tenant.Status = TenantStatuses.Failed;
            tenantSubscription.Status = TenantSubscriptionStatuses.Failed;
            _ = await tenantRepository.UpdateAsync(tenant);
            _ = await tenantSubscriptionRepository.UpdateAsync(tenantSubscription);
            return Result.BadRequest<ActivateTenantSubscriptionResult>(SignupMessages.PaymentSetupFailed);
        }

        tenant.Status = tenantSubscription.Status switch
        {
            TenantSubscriptionStatuses.Active => TenantStatuses.Active,
            TenantSubscriptionStatuses.PastDue => TenantStatuses.PastDue,
            _ => TenantStatuses.PendingPayment,
        };
        _ = await tenantRepository.UpdateAsync(tenant);

        return Result.Created(new ActivateTenantSubscriptionResult(tenant.Id, clientSecret), "Tenant subscription activated successfully.");
    }
}
