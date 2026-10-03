using System.Text.Json.Serialization;
using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Contracts;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.GetInvoiceById;
using ResultHandler.Core.Base;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices.GetInvoiceForStaff;

public record GetInvoiceForStaffQuery(Guid Id) : IRequest<OperationDataResult<InvoiceResponse>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [CommerceOperationClaims.Admin, CommerceOperationClaims.Read, CommerceOperationClaims.Write];
}

public class GetInvoiceForStaffHandler(ISender sender) : IRequestHandler<GetInvoiceForStaffQuery, OperationDataResult<InvoiceResponse>>
{
    public Task<OperationDataResult<InvoiceResponse>> Handle(GetInvoiceForStaffQuery request, CancellationToken cancellationToken) =>
        sender.Send(new GetInvoiceByIdQuery(request.Id, null), cancellationToken);
}
