using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Constants;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Contracts;
using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.SharedKernel.Payments;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices.PayInvoice;

/// <summary>Starts (or resumes, via the idempotency key) the card payment for the customer's own open invoice. A provider that settles
/// synchronously marks it paid right away; otherwise the client confirms with ClientSecret and the webhook marks it paid.</summary>
public record PayInvoiceCommand(Guid Id, Guid UserId) : IRequest<OperationDataResult<PayInvoiceResult>>;

public class PayInvoiceHandler(IInvoiceRepository repository, IPaymentGateway paymentGateway, IInvoiceService invoiceService) : IRequestHandler<PayInvoiceCommand, OperationDataResult<PayInvoiceResult>>
{
    public async Task<OperationDataResult<PayInvoiceResult>> Handle(PayInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await repository.GetAsync(i => i.Id == request.Id && i.UserId == request.UserId, cancellationToken: cancellationToken);

        if (invoice is null)
        {
            return Result.NotFound<PayInvoiceResult>(InvoiceMessages.NotFound);
        }

        if (!invoice.IsOpen)
        {
            return Result.Conflict<PayInvoiceResult>(InvoiceMessages.OnlyOpenCanBePaid);
        }

        if (invoice.Total <= 0)
        {
            return Result.BadRequest<PayInvoiceResult>(InvoiceMessages.NothingToPay);
        }

        PaymentIntentResult intent;

        try
        {
            intent = await paymentGateway
                .CreatePaymentIntentAsync(new CreatePaymentIntentRequest(invoice.Total, invoice.Currency.ToLowerInvariant(), invoice.Id.ToString(), invoice.BillToEmail, $"invoice-pay:{invoice.Id}:{invoice.Total:0.00}"), cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            return Result.BadGateway<PayInvoiceResult>(InvoiceMessages.PaymentSetupFailed);
        }

        invoice.StartCardPayment(paymentGateway.ProviderName, intent.ProviderPaymentId);

        _ = await repository.UpdateAsync(invoice, cancellationToken: cancellationToken);

        if (intent.Status == PaymentStatuses.Succeeded)
        {
            await invoiceService.MarkPaidAsync(invoice, InvoicePaymentMethods.Card, paymentGateway.ProviderName, intent.ProviderPaymentId, cancellationToken);

            return Result.Success(new PayInvoiceResult(true, null), "Payment received. Thank you!");
        }

        return Result.Success(new PayInvoiceResult(false, intent.ClientSecret), "Payment started.");
    }
}
