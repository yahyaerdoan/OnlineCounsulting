using OnlineConsulting.Modules.Commerce.Domain;
using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Constants;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices.VoidInvoice;

public record VoidInvoiceCommand(Guid Id, string? Reason) : IRequest<OperationResult>, ISecureAddRequest
{
    public string[] Roles => [CommerceOperationClaims.Admin, CommerceOperationClaims.Write, CommerceOperationClaims.Update];
}

public class VoidInvoiceHandler(IInvoiceRepository repository, IInvoiceService invoiceService) : IRequestHandler<VoidInvoiceCommand, OperationResult>
{
    public async Task<OperationResult> Handle(VoidInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await repository.GetWithLinesAsync(i => i.Id == request.Id, cancellationToken: cancellationToken);

        if (invoice is null)
        {
            return Result.NotFound(InvoiceMessages.NotFound);
        }

        if (!invoice.IsOpen)
        {
            return Result.Conflict(InvoiceMessages.OnlyOpenCanBeVoided);
        }

        await invoiceService.VoidAsync(invoice, request.Reason, cancellationToken);

        return Result.Success($"{invoice.InvoiceNumber} voided.");
    }
}
