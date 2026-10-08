using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Constants;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices.GetInvoiceById;

/// <summary>OwnerUserId set means a customer asking - they only ever see their own invoice; null is the staff path (see GetInvoiceForStaffQuery).</summary>
public record GetInvoiceByIdQuery(Guid Id, Guid? OwnerUserId) : IRequest<OperationDataResult<InvoiceResponse>>;

public class GetInvoiceByIdHandler(IInvoiceRepository repository) : IRequestHandler<GetInvoiceByIdQuery, OperationDataResult<InvoiceResponse>>
{
    public async Task<OperationDataResult<InvoiceResponse>> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        var invoice = await repository.GetWithLinesAsync(i => i.Id == request.Id && (request.OwnerUserId == null || i.UserId == request.OwnerUserId), enableTracking: false, cancellationToken: cancellationToken);
        return invoice is null
            ? Result.NotFound<InvoiceResponse>(InvoiceMessages.NotFound)
            : Result.Success(InvoiceResponse.FromDomain(invoice), "Invoice retrieved successfully.");
    }
}
