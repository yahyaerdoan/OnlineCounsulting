using Core.ApplicationLayer.Requests.Lists;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.Contracts;
using OnlineConsulting.Modules.SiteContent.Domain.Service;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.ListServiceAreas;

public record ListServiceAreasQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null) : IRequest<OperationDataResult<Paginate<ServiceAreaResponse>>>, IDynamicListRequest
{
    public static IReadOnlySet<string> QueryableFields { get; } = new HashSet<string>([nameof(ServiceArea.Id), nameof(ServiceArea.Name), nameof(ServiceArea.State)]);
}

public class ListServiceAreasHandler(IServiceAreaRepository repository)
    : IRequestHandler<ListServiceAreasQuery, OperationDataResult<Paginate<ServiceAreaResponse>>>
{
    public async Task<OperationDataResult<Paginate<ServiceAreaResponse>>> Handle(ListServiceAreasQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request, defaultOrderBy: x => x.DisplayOrder, tieBreaker: x => x.Id, cancellationToken: cancellationToken);

        var response = new Paginate<ServiceAreaResponse>
        {
            Items = [.. paged.Items.Select(ServiceAreaResponse.FromDomain)],
            Index = paged.Index,
            Size = paged.Size,
            Count = paged.Count,
            Pages = paged.Pages,
        };

        return Result.Success(response, "Service areas retrieved successfully.");
    }
}
