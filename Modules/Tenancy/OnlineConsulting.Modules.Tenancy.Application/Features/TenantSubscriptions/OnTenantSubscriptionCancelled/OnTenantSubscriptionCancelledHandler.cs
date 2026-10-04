using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Payments;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.OnTenantSubscriptionCancelled;

/// <summary>Handles provider-initiated cancellation (e.g. Stripe dashboard) - no-ops if already Cancelled, cascades to Tenant.Status = Suspended.</summary>
public class OnTenantSubscriptionCancelledHandler(ITenantSubscriptionRepository subscriptionRepository, ITenantRepository tenantRepository) : INotificationHandler<SubscriptionCancelledNotification>
{
    public async Task Handle(SubscriptionCancelledNotification notification, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(notification.ReferenceId, out var tenantSubscriptionId))
        {
            return;
        }

        var tenantSubscription = await subscriptionRepository.GetAsync(s => s.Id == tenantSubscriptionId, cancellationToken: cancellationToken);
        if (tenantSubscription is null || tenantSubscription.IsCancelled)
        {
            return;
        }

        tenantSubscription.Cancel();
        _ = await subscriptionRepository.UpdateAsync(tenantSubscription);

        var tenant = await tenantRepository.GetAsync(t => t.Id == tenantSubscription.TenantId, cancellationToken: cancellationToken);
        if (tenant is null || tenant.IsHeldByStaff)
        {
            return;
        }

        tenant.ApplySubscriptionEnded();
        _ = await tenantRepository.UpdateAsync(tenant);
    }
}
