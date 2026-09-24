using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Payments;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.OnTenantSubscriptionRenewed;

/// <summary>ReferenceId round-trips to a TenantSubscription id set by ActivateTenantSubscriptionHandler; Memberships has its own handler on the same broadcast, so an unrecognized id here is an expected no-op. Also restores Tenant.Status to Active (unless Suspended/Cancelled) after a PastDue recovery.</summary>
public class OnTenantSubscriptionRenewedHandler(ITenantSubscriptionRepository subscriptionRepository, ITenantRepository tenantRepository) : INotificationHandler<SubscriptionRenewedNotification>
{
    public async Task Handle(SubscriptionRenewedNotification notification, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(notification.ReferenceId, out var tenantSubscriptionId))
        {
            return;
        }

        var tenantSubscription = await subscriptionRepository.GetAsync(s => s.Id == tenantSubscriptionId, cancellationToken: cancellationToken);
        if (tenantSubscription is null)
        {
            return;
        }

        tenantSubscription.RenewalDate = notification.CurrentPeriodEnd.UtcDateTime;
        tenantSubscription.Status = TenantSubscriptionStatuses.Active;
        _ = await subscriptionRepository.UpdateAsync(tenantSubscription);

        var tenant = await tenantRepository.GetAsync(t => t.Id == tenantSubscription.TenantId, cancellationToken: cancellationToken);
        if (tenant is null || tenant.Status is TenantStatuses.Suspended or TenantStatuses.Cancelled)
        {
            return;
        }

        tenant.Status = TenantStatuses.Active;
        _ = await tenantRepository.UpdateAsync(tenant);
    }
}
