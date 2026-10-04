using Core.ApplicationLayer.Requests.Lists;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Services.Application.Features.Services.Abstractions;
using OnlineConsulting.Modules.Services.Application.Features.Services.Contracts;
using OnlineConsulting.Modules.Services.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Services.Application.Features.Services.ListServices;

/// <summary>Sortable/filterable variant of GetServicesQuery for the admin ServerDataTable.</summary>
public record ListServicesQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null) : IRequest<OperationDataResult<Paginate<ServiceResponse>>>, IDynamicListRequest
{
    public static IReadOnlySet<string> QueryableFields { get; } = new HashSet<string>([nameof(Service.Title), nameof(Service.Description)]);
}

public class ListServicesHandler(IServiceRepository repository)
    : IRequestHandler<ListServicesQuery, OperationDataResult<Paginate<ServiceResponse>>>
{
    public async Task<OperationDataResult<Paginate<ServiceResponse>>> Handle(ListServicesQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request, defaultOrderBy: s => s.Title, tieBreaker: s => s.Id, cancellationToken: cancellationToken);

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
