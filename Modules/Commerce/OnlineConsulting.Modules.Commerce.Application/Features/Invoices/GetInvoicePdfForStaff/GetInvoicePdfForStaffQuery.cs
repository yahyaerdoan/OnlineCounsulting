using System.Text.Json.Serialization;
using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Contracts;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.GetInvoicePdf;
using ResultHandler.Core.Base;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices.GetInvoicePdfForStaff;

public record GetInvoicePdfForStaffQuery(Guid Id) : IRequest<OperationDataResult<InvoicePdfResponse>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [CommerceOperationClaims.Admin, CommerceOperationClaims.Read, CommerceOperationClaims.Write];
}

public class GetInvoicePdfForStaffHandler(ISender sender) : IRequestHandler<GetInvoicePdfForStaffQuery, OperationDataResult<InvoicePdfResponse>>
{
    public Task<OperationDataResult<InvoicePdfResponse>> Handle(GetInvoicePdfForStaffQuery request, CancellationToken cancellationToken) =>
        sender.Send(new GetInvoicePdfQuery(request.Id, null), cancellationToken);
}
