using MediatR;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Abstractions;
using OnlineConsulting.Modules.Memberships.Domain;
using OnlineConsulting.SharedKernel.Payments;

namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.OnSubscriptionRenewed;

/// <summary>ReferenceId round-trips the CustomerMembership id set by SubscribeToMembershipHandler, so lookup is by Id, not ProviderSubscriptionId.
/// A membership still waiting for its first payment (card confirmed in the browser) starts here: receipt email plus "welcome" notification.
/// One that started at subscribe time was receipted there, so its first invoice is skipped; later renewals get a receipt and a "renewed"
/// notification.</summary>
public class OnSubscriptionRenewedHandler(ICustomerMembershipRepository repository, MembershipReceiptSender receiptSender, IMembershipNotifier notifier) : INotificationHandler<SubscriptionRenewedNotification>
{
    public async Task Handle(SubscriptionRenewedNotification notification, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(notification.ReferenceId, out var membershipId))
        {
            return;
        }

        var membership = await repository.GetAsync(m => m.Id == membershipId, cancellationToken: cancellationToken);

        if (membership is null || membership.IsCancelled)
        {
            return;
        }

        var starting = membership.IsAwaitingFirstPayment;

        membership.Renew(notification.CurrentPeriodEnd);

        _ = await repository.UpdateAsync(membership, cancellationToken: cancellationToken);

        var renewal = notification.Invoice is { } renewedInvoice && renewedInvoice.BillingReason != SubscriptionInvoice.FirstInvoiceReason;

        if (notification.Invoice is { IsPaid: true } invoice && (starting || renewal))
        {
            await receiptSender.SendAsync(membership, invoice, cancellationToken);
        }

        if (starting)
        {
            await notifier.StartedAsync(membership, cancellationToken);
        }
        else if (renewal)
        {
            await notifier.RenewedAsync(membership, cancellationToken);
        }
    }
}
