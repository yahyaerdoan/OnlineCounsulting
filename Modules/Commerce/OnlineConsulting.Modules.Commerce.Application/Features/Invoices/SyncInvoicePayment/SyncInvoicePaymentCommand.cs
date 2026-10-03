using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Constants;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Contracts;
using OnlineConsulting.SharedKernel.Payments;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices.SyncInvoicePayment;

/// <summary>
/// Asks the provider about the customer's own open invoice right after they paid it (and whenever they open it), instead of waiting for the
/// webhook. A charge that already succeeded is published exactly like the webhook would, so OnInvoicePaymentStatusChangedHandler marks the
/// invoice paid and sends the receipt and notification once, whichever arrives first.
/// </summary>
public record SyncInvoicePaymentCommand(Guid Id, Guid UserId) : IRequest<OperationDataResult<SyncInvoicePaymentResult>>;

public class SyncInvoicePaymentHandler(IInvoiceRepository repository, IPaymentGateway paymentGateway, IPublisher publisher)
    : IRequestHandler<SyncInvoicePaymentCommand, OperationDataResult<SyncInvoicePaymentResult>>
{
    public async Task<OperationDataResult<SyncInvoicePaymentResult>> Handle(SyncInvoicePaymentCommand request, CancellationToken cancellationToken)
    {
        var invoice = await repository.GetAsync(i => i.Id == request.Id && i.UserId == request.UserId, enableTracking: false, cancellationToken: cancellationToken);
        if (invoice is null)
        {
            return Result.NotFound<SyncInvoicePaymentResult>(InvoiceMessages.NotFound);
        }

        if (invoice.Status != InvoiceStatuses.Open || invoice.ProviderPaymentId is not { } providerPaymentId || invoice.PaymentProvider != paymentGateway.ProviderName)
        {
            return Result.Success(new SyncInvoicePaymentResult(invoice.Status, invoice.Status == InvoiceStatuses.Paid), "Invoice status unchanged.");
        }

        var (failure, status) = await PaymentGatewayCall.RunWithResultAsync(() => paymentGateway.GetStatusAsync(providerPaymentId, cancellationToken),
            InvoiceMessages.PaymentSetupFailed);
        if (failure is not null || status is null)
        {
            return Result.BadGateway<SyncInvoicePaymentResult>(failure?.Detail ?? InvoiceMessages.PaymentSetupFailed);
        }

        if (status.Status != PaymentStatuses.Succeeded)
        {
            return Result.Success(new SyncInvoicePaymentResult(invoice.Status, false), "Payment not completed yet.");
        }

        await publisher.Publish(new PaymentSucceededNotification(invoice.Id.ToString(), providerPaymentId, paymentGateway.ProviderName), cancellationToken);
        return Result.Success(new SyncInvoicePaymentResult(InvoiceStatuses.Paid, true), "Payment received. Thank you!");
    }
}
