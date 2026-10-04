using Core.ApplicationLayer.Requests.Lists;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceOfferings.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceOfferings.Contracts;
using OnlineConsulting.Modules.SiteContent.Domain.Service;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.ServiceOfferings.ListServiceOfferings;

/// <summary>Sortable/filterable variant of GetAllServiceOfferingsQuery for the admin ServerDataTable.</summary>
public record ListServiceOfferingsQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null) : IRequest<OperationDataResult<Paginate<ServiceOfferingResponse>>>, IDynamicListRequest
{
    public static IReadOnlySet<string> QueryableFields { get; } = new HashSet<string>([nameof(ServiceOffering.Title), nameof(ServiceOffering.Description)]);
}

public class ListServiceOfferingsHandler(IServiceOfferingRepository repository)
    : IRequestHandler<ListServiceOfferingsQuery, OperationDataResult<Paginate<ServiceOfferingResponse>>>
{
    public async Task<OperationDataResult<Paginate<ServiceOfferingResponse>>> Handle(ListServiceOfferingsQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request, defaultOrderBy: x => x.DisplayOrder, tieBreaker: x => x.Id, cancellationToken: cancellationToken);

        var response = new Paginate<ServiceOfferingResponse>
        {
            Items = [.. paged.Items.Select(ServiceOfferingResponse.FromDomain)],
            Index = paged.Index,
            Size = paged.Size,
            Count = paged.Count,
            Pages = paged.Pages,
        };

        return Result.Success(response, "Service offerings retrieved successfully.");
    }
}
