using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.GetAllServiceAreas;

public record GetAllServiceAreasQuery : IRequest<OperationDataResult<List<ServiceAreaResponse>>>;

public class GetAllServiceAreasHandler(IServiceAreaRepository repository) : IRequestHandler<GetAllServiceAreasQuery, OperationDataResult<List<ServiceAreaResponse>>>
{
    public async Task<OperationDataResult<List<ServiceAreaResponse>>> Handle(GetAllServiceAreasQuery request, CancellationToken cancellationToken)
    {
        var entities = await repository.GetListAsync(orderBy: q => q.OrderBy(x => x.DisplayOrder), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        var response = entities.Items.Select(ServiceAreaResponse.FromDomain).ToList();

        return Result.Success(response, "Service areas retrieved successfully.");
    }
}
