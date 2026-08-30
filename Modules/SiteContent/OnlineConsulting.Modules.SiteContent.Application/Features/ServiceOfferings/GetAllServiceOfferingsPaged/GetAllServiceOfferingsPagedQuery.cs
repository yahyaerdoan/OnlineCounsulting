using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceOfferings.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceOfferings.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.ServiceOfferings.GetAllServiceOfferingsPaged;

/// <summary>Sortable/filterable variant of GetAllServiceOfferingsQuery for the admin ServerDataTable.</summary>
public record GetAllServiceOfferingsPagedQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null) : IRequest<OperationDataResult<Paginate<ServiceOfferingResponse>>>;

public class GetAllServiceOfferingsPagedHandler(IServiceOfferingRepository repository)
    : IRequestHandler<GetAllServiceOfferingsPagedQuery, OperationDataResult<Paginate<ServiceOfferingResponse>>>
{
    public async Task<OperationDataResult<Paginate<ServiceOfferingResponse>>> Handle(GetAllServiceOfferingsPagedQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request.PageRequest, request.DynamicQuery, defaultOrderBy: x => x.DisplayOrder, tieBreaker: x => x.Id, cancellationToken);

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
