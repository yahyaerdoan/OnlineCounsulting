using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Payments;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.OnTenantSubscriptionCancelled;

/// <summary>Handles cancellation initiated on the provider side (e.g. from the Stripe dashboard) - no-ops if already Cancelled. Cascades to Tenant.Status = Suspended, which is what TenantStatusCheckBehavior (a later phase) will gate access on.</summary>
public class OnTenantSubscriptionCancelledHandler(ITenantSubscriptionRepository subscriptionRepository, ITenantRepository tenantRepository) : INotificationHandler<SubscriptionCancelledNotification>
{
    public async Task Handle(SubscriptionCancelledNotification notification, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(notification.ReferenceId, out var tenantSubscriptionId))
            return;

        var tenantSubscription = await subscriptionRepository.GetAsync(s => s.Id == tenantSubscriptionId, cancellationToken: cancellationToken);
        if (tenantSubscription is null || tenantSubscription.Status == TenantSubscriptionStatuses.Cancelled)
            return;

        tenantSubscription.Status = TenantSubscriptionStatuses.Cancelled;
        await subscriptionRepository.UpdateAsync(tenantSubscription);

        var tenant = await tenantRepository.GetAsync(t => t.Id == tenantSubscription.TenantId, cancellationToken: cancellationToken);
        if (tenant is null || tenant.Status == TenantStatuses.Suspended)
            return;

        tenant.Status = TenantStatuses.Suspended;
        await tenantRepository.UpdateAsync(tenant);
    }
}
