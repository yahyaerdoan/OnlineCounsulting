using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Payments;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.OnTenantSubscriptionRenewed;

/// <summary>ReferenceId round-trips to a TenantSubscription id set by ActivateTenantSubscriptionHandler; Memberships has its own handler on the same broadcast, so an unrecognized id here is an expected no-op. Also restores Tenant.Status to Active (unless Suspended/Cancelled) after a PastDue recovery.
/// Renewal invoices also get a receipt email; the first invoice (subscription_create) is receipted by the signup itself.</summary>
public class OnTenantSubscriptionRenewedHandler(ITenantSubscriptionRepository subscriptionRepository, ITenantRepository tenantRepository, TenantReceiptSender receiptSender)
    : INotificationHandler<SubscriptionRenewedNotification>
{
    public async Task Handle(SubscriptionRenewedNotification notification, CancellationToken cancellationToken)
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

        tenantSubscription.Renew(notification.CurrentPeriodEnd.UtcDateTime);
        _ = await subscriptionRepository.UpdateAsync(tenantSubscription, cancellationToken: cancellationToken);

        var tenant = await tenantRepository.GetAsync(t => t.Id == tenantSubscription.TenantId, cancellationToken: cancellationToken);
        if (tenant is not null && notification.Invoice is { IsPaid: true } invoice && invoice.BillingReason != SubscriptionInvoice.FirstInvoiceReason)
        {
            await receiptSender.SendAsync(tenant, invoice, cancellationToken);
        }

        if (tenant is null || tenant.IsHeldByStaff)
        {
            return;
        }

        tenant.ApplyRenewal();
        _ = await tenantRepository.UpdateAsync(tenant, cancellationToken: cancellationToken);
    }
}
