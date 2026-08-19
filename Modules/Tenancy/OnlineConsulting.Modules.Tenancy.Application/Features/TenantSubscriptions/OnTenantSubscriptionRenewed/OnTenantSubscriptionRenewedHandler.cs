using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Payments;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.OnTenantSubscriptionRenewed;

/// <summary>ReferenceId is opaque to the webhook - only handled here because ActivateTenantSubscriptionHandler set CreateSubscriptionRequest.ReferenceId to a TenantSubscription id, so it round-trips to a lookup by Id. The same broadcast notification also reaches Memberships' own handler - each ignores ids it doesn't recognize, so a no-op TryParse/lookup miss here is expected, not an error. Also brings Tenant.Status back to Active (unless Suspended/Cancelled) - a renewal is the natural recovery path for a tenant that had gone PastDue on a failed payment.</summary>
public class OnTenantSubscriptionRenewedHandler(ITenantSubscriptionRepository subscriptionRepository, ITenantRepository tenantRepository) : INotificationHandler<SubscriptionRenewedNotification>
{
    public async Task Handle(SubscriptionRenewedNotification notification, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(notification.ReferenceId, out var tenantSubscriptionId))
            return;

        var tenantSubscription = await subscriptionRepository.GetAsync(s => s.Id == tenantSubscriptionId, cancellationToken: cancellationToken);
        if (tenantSubscription is null)
            return;

        tenantSubscription.RenewalDate = notification.CurrentPeriodEnd.UtcDateTime;
        tenantSubscription.Status = TenantSubscriptionStatuses.Active;
        await subscriptionRepository.UpdateAsync(tenantSubscription);

        var tenant = await tenantRepository.GetAsync(t => t.Id == tenantSubscription.TenantId, cancellationToken: cancellationToken);
        if (tenant is null || tenant.Status is TenantStatuses.Suspended or TenantStatuses.Cancelled)
            return;

        tenant.Status = TenantStatuses.Active;
        await tenantRepository.UpdateAsync(tenant);
    }
}
