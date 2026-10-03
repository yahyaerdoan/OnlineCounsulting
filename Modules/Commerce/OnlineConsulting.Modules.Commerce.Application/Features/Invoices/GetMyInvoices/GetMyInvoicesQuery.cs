using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices.GetMyInvoices;

/// <summary>The signed-in customer's own invoices, newest first.</summary>
public record GetMyInvoicesQuery(Guid UserId) : IRequest<OperationDataResult<List<InvoiceResponse>>>;

public class GetMyInvoicesHandler(IInvoiceRepository repository) : IRequestHandler<GetMyInvoicesQuery, OperationDataResult<List<InvoiceResponse>>>
{
    public async Task<OperationDataResult<List<InvoiceResponse>>> Handle(GetMyInvoicesQuery request, CancellationToken cancellationToken)
    {
        var invoices = await repository.GetListAsync(i => i.UserId == request.UserId, orderBy: q => q.OrderByDescending(i => i.IssuedAt),
            size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        return Result.Success<List<InvoiceResponse>>([.. invoices.Items.Select(i => InvoiceResponse.FromDomain(i))], "Invoices retrieved successfully.");
    }
}
