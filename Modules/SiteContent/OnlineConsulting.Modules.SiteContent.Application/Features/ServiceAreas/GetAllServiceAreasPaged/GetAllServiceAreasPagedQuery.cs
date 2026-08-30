using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.GetAllServiceAreasPaged;

public record GetAllServiceAreasPagedQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null) : IRequest<OperationDataResult<Paginate<ServiceAreaResponse>>>;

public class GetAllServiceAreasPagedHandler(IServiceAreaRepository repository)
    : IRequestHandler<GetAllServiceAreasPagedQuery, OperationDataResult<Paginate<ServiceAreaResponse>>>
{
    public async Task<OperationDataResult<Paginate<ServiceAreaResponse>>> Handle(GetAllServiceAreasPagedQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request.PageRequest, request.DynamicQuery, defaultOrderBy: x => x.DisplayOrder, tieBreaker: x => x.Id, cancellationToken);

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
