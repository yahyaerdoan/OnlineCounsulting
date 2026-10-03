using System.Text.Json.Serialization;
using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Constants;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices.MarkInvoicePaid;

/// <summary>Staff record an offline payment (cash, check, or a card taken on site).</summary>
public record MarkInvoicePaidCommand(Guid Id, string PaymentMethod) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [CommerceOperationClaims.Admin, CommerceOperationClaims.Write, CommerceOperationClaims.Update];
}

public class MarkInvoicePaidHandler(IInvoiceRepository repository, IInvoiceService invoiceService) : IRequestHandler<MarkInvoicePaidCommand, OperationResult>
{
    public async Task<OperationResult> Handle(MarkInvoicePaidCommand request, CancellationToken cancellationToken)
    {
        var invoice = await repository.GetAsync(i => i.Id == request.Id, cancellationToken: cancellationToken);
        if (invoice is null)
        {
            return Result.NotFound(InvoiceMessages.NotFound);
        }

        if (invoice.Status != InvoiceStatuses.Open)
        {
            return Result.Conflict(InvoiceMessages.OnlyOpenCanBePaid);
        }

        var method = InvoicePaymentMethods.Offline.Contains(request.PaymentMethod) ? request.PaymentMethod : InvoicePaymentMethods.Cash;
        await invoiceService.MarkPaidAsync(invoice, method, null, null, cancellationToken);
        return Result.Success($"{invoice.InvoiceNumber} marked as paid. The customer got a receipt.");
    }
}
