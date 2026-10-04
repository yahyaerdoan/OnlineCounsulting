using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Constants;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Contracts;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.GetInvoiceById;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices.GetInvoicePdf;

/// <summary>Same ownership rule as GetInvoiceByIdQuery. Returned base64 inside the usual envelope so web and the app download it the same way.</summary>
public record GetInvoicePdfQuery(Guid Id, Guid? OwnerUserId) : IRequest<OperationDataResult<InvoicePdfResponse>>;

public class GetInvoicePdfHandler(ISender sender, IInvoicePdfRenderer renderer, InvoiceBusinessInfo business, ITenantProvider tenantProvider,
    ITenantTimeZoneReader timeZoneReader)
    : IRequestHandler<GetInvoicePdfQuery, OperationDataResult<InvoicePdfResponse>>
{
    public async Task<OperationDataResult<InvoicePdfResponse>> Handle(GetInvoicePdfQuery request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetInvoiceByIdQuery(request.Id, request.OwnerUserId), cancellationToken);
        if (!result.IsSuccessful || result.Data is not { } invoice)
        {
            return Result.NotFound<InvoicePdfResponse>(InvoiceMessages.NotFound);
        }

        var bytes = renderer.Render(invoice, business, await timeZoneReader.GetAsync(tenantProvider.TenantId, cancellationToken));
        return Result.Success(new InvoicePdfResponse($"{invoice.InvoiceNumber}.pdf", Convert.ToBase64String(bytes)), "Invoice PDF ready.");
    }
}
