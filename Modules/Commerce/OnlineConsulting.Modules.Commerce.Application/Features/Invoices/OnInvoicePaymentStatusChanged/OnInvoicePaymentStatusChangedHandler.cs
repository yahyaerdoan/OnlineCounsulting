using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Abstractions;
using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.SharedKernel.Payments;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices.OnInvoicePaymentStatusChanged;

/// <summary>ReferenceId is the invoice id; anything else belongs to another module's handler (orders, memberships) and is ignored here.</summary>
public class OnInvoicePaymentStatusChangedHandler(IInvoiceRepository repository, IInvoiceService invoiceService) : INotificationHandler<PaymentSucceededNotification>
{
    public async Task Handle(PaymentSucceededNotification notification, CancellationToken cancellationToken)
    {
        if (await FindAsync(notification.ReferenceId, notification.ProviderPaymentId, cancellationToken) is { IsOpen: true } invoice)
        {
            await invoiceService.MarkPaidAsync(invoice, InvoicePaymentMethods.Card, notification.ProviderName, notification.ProviderPaymentId, cancellationToken);
        }
    }

    private async Task<Invoice?> FindAsync(string referenceId, string providerPaymentId, CancellationToken cancellationToken) =>
        Guid.TryParse(referenceId, out var invoiceId)
            ? await repository.GetAsync(i => i.Id == invoiceId && i.ProviderPaymentId == providerPaymentId, cancellationToken: cancellationToken)
            : null;
}
