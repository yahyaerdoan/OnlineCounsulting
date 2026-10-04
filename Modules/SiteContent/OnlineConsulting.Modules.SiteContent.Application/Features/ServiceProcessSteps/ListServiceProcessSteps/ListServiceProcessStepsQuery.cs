using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceProcessSteps.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceProcessSteps.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.ServiceProcessSteps.ListServiceProcessSteps;

/// <summary>Sortable/filterable variant of GetAllServiceProcessStepsQuery for the admin ServerDataTable.</summary>
public record ListServiceProcessStepsQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null) : IRequest<OperationDataResult<Paginate<ServiceProcessStepResponse>>>;

public class ListServiceProcessStepsHandler(IServiceProcessStepRepository repository)
    : IRequestHandler<ListServiceProcessStepsQuery, OperationDataResult<Paginate<ServiceProcessStepResponse>>>
{
    public async Task<OperationDataResult<Paginate<ServiceProcessStepResponse>>> Handle(ListServiceProcessStepsQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request.PageRequest, request.DynamicQuery, defaultOrderBy: x => x.DisplayOrder, tieBreaker: x => x.Id, cancellationToken);

        var response = new Paginate<ServiceProcessStepResponse>
        {
            Items = [.. paged.Items.Select(ServiceProcessStepResponse.FromDomain)],
            Index = paged.Index,
            Size = paged.Size,
            Count = paged.Count,
            Pages = paged.Pages,
        };

        return Result.Success(response, "Service process steps retrieved successfully.");
    }
}
