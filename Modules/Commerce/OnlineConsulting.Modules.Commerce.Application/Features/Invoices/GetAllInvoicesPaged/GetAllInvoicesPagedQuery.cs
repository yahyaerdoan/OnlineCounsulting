using System.Text.Json.Serialization;
using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices.GetAllInvoicesPaged;

public record GetAllInvoicesPagedQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null) : IRequest<OperationDataResult<Paginate<InvoiceResponse>>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [CommerceOperationClaims.Admin, CommerceOperationClaims.Read, CommerceOperationClaims.Write];
}

public class GetAllInvoicesPagedHandler(IInvoiceRepository repository) : IRequestHandler<GetAllInvoicesPagedQuery, OperationDataResult<Paginate<InvoiceResponse>>>
{
    public async Task<OperationDataResult<Paginate<InvoiceResponse>>> Handle(GetAllInvoicesPagedQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request.PageRequest, request.DynamicQuery, defaultOrderBy: i => i.IssuedAt, tieBreaker: i => i.Id,
            cancellationToken, defaultDescending: true);

        return Result.Success(new Paginate<InvoiceResponse>
        {
            Items = [.. paged.Items.Select(i => InvoiceResponse.FromDomain(i))],
            Index = paged.Index,
            Size = paged.Size,
            Count = paged.Count,
            Pages = paged.Pages,
        }, "Invoices retrieved successfully.");
    }
}
