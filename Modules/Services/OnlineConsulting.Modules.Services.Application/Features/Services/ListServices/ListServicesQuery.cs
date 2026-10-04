using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Services.Application.Features.Services.Abstractions;
using OnlineConsulting.Modules.Services.Application.Features.Services.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Services.Application.Features.Services.ListServices;

/// <summary>Sortable/filterable variant of GetServicesQuery for the admin ServerDataTable.</summary>
public record ListServicesQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null) : IRequest<OperationDataResult<Paginate<ServiceResponse>>>;

public class ListServicesHandler(IServiceRepository repository)
    : IRequestHandler<ListServicesQuery, OperationDataResult<Paginate<ServiceResponse>>>
{
    public async Task<OperationDataResult<Paginate<ServiceResponse>>> Handle(ListServicesQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request.PageRequest, request.DynamicQuery, defaultOrderBy: s => s.Title, tieBreaker: s => s.Id, cancellationToken);

        var response = new Paginate<ServiceResponse>
        {
            Items = [.. paged.Items.Select(s => ServiceResponse.FromDomain(s))],
            Index = paged.Index,
            Size = paged.Size,
            Count = paged.Count,
            Pages = paged.Pages,
        };

        return Result.Success(response, "Services retrieved successfully.");
    }
}
