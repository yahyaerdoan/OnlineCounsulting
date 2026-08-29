using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Services.Application.Abstractions;
using OnlineConsulting.Modules.Services.Application.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Services.Application.Features.GetAllServicesPaged;

/// <summary>Sortable/filterable variant of GetServicesQuery for the admin ServerDataTable.</summary>
public record GetAllServicesPagedQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null) : IRequest<OperationDataResult<Paginate<ServiceResponse>>>;

public class GetAllServicesPagedHandler(IServiceRepository repository)
    : IRequestHandler<GetAllServicesPagedQuery, OperationDataResult<Paginate<ServiceResponse>>>
{
    public async Task<OperationDataResult<Paginate<ServiceResponse>>> Handle(GetAllServicesPagedQuery request, CancellationToken cancellationToken)
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
