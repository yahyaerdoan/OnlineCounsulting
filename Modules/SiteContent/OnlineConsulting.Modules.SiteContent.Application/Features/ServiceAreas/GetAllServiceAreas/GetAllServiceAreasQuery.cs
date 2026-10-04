using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.GetAllServiceAreas;

public record GetAllServiceAreasQuery : IRequest<OperationDataResult<List<ServiceAreaResponse>>>;

public class GetAllServiceAreasHandler(IServiceAreaRepository repository) : IRequestHandler<GetAllServiceAreasQuery, OperationDataResult<List<ServiceAreaResponse>>>
{
    public async Task<OperationDataResult<List<ServiceAreaResponse>>> Handle(GetAllServiceAreasQuery request, CancellationToken cancellationToken)
    {
        var entities = await repository.GetAllAsync(orderBy: q => q.OrderBy(x => x.DisplayOrder), cancellationToken: cancellationToken);
        var response = entities.Select(ServiceAreaResponse.FromDomain).ToList();

        return Result.Success(response, "Service areas retrieved successfully.");
    }
}
