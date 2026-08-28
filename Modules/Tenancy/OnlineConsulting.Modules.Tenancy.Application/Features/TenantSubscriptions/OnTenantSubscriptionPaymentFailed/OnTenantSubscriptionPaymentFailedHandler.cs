using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Payments;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.OnTenantSubscriptionPaymentFailed;

/// <summary>Sets PastDue only - does not suspend on a single failed payment, only an actual cancellation (OnTenantSubscriptionCancelledHandler) does. Notifies via email (Tenant.PrimaryContactEmail), not push - Tenancy has no UserId to target and deliberately avoids depending on Identity's User type (see ReserveTenantCommand's doc comment on the same boundary decision).</summary>
public class OnTenantSubscriptionPaymentFailedHandler(ITenantSubscriptionRepository subscriptionRepository, ITenantRepository tenantRepository, IEmailOutboxWriter<ITenancyOutboxModule> outboxWriter)
    : INotificationHandler<SubscriptionPaymentFailedNotification>
{
    public async Task Handle(SubscriptionPaymentFailedNotification notification, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(notification.ReferenceId, out var tenantSubscriptionId))
        {
            return;
        }

        var tenantSubscription = await subscriptionRepository.GetAsync(s => s.Id == tenantSubscriptionId, cancellationToken: cancellationToken);
        if (tenantSubscription is null || tenantSubscription.Status == TenantSubscriptionStatuses.Cancelled)
        {
            return;
        }

        tenantSubscription.Status = TenantSubscriptionStatuses.PastDue;
        _ = await subscriptionRepository.UpdateAsync(tenantSubscription);

        var tenant = await tenantRepository.GetAsync(t => t.Id == tenantSubscription.TenantId, cancellationToken: cancellationToken);
        if (tenant is null || tenant.Status is TenantStatuses.Suspended or TenantStatuses.Cancelled)
        {
            return;
        }

        tenant.Status = TenantStatuses.PastDue;

        // Enqueue before UpdateAsync - UpdateAsync's own SaveChangesAsync is what flushes this staged row.
        outboxWriter.Enqueue(
            tenant.PrimaryContactEmail,
            "Payment failed for your subscription",
            $"We couldn't process your latest subscription payment for {tenant.Name}. Please update your payment method to avoid losing access to your purchased modules.",
            sourceReference: $"Tenant:{tenant.Id}");

        _ = await tenantRepository.UpdateAsync(tenant);
    }
}
