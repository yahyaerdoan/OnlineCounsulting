using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Requests.Lists;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Contracts;
using OnlineConsulting.Modules.Commerce.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices.ListInvoices;

public record ListInvoicesQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null) : IRequest<OperationDataResult<Paginate<InvoiceResponse>>>, ISecureAddRequest, IDynamicListRequest
{
    public static IReadOnlySet<string> QueryableFields { get; } = new HashSet<string>([nameof(Invoice.IssuedAt)]);

    public string[] Roles => [CommerceOperationClaims.Admin, CommerceOperationClaims.Read, CommerceOperationClaims.Write];
}

public class ListInvoicesHandler(IInvoiceRepository repository) : IRequestHandler<ListInvoicesQuery, OperationDataResult<Paginate<InvoiceResponse>>>
{
    public async Task<OperationDataResult<Paginate<InvoiceResponse>>> Handle(ListInvoicesQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request, defaultOrderBy: i => i.IssuedAt, tieBreaker: i => i.Id, defaultDescending: true, cancellationToken: cancellationToken);

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
