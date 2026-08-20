using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.GetServiceAreaBySlug;

public record GetServiceAreaBySlugQuery(string Slug) : IRequest<OperationDataResult<ServiceAreaResponse>>;

public class GetServiceAreaBySlugHandler(IServiceAreaRepository repository) : IRequestHandler<GetServiceAreaBySlugQuery, OperationDataResult<ServiceAreaResponse>>
{
    public async Task<OperationDataResult<ServiceAreaResponse>> Handle(GetServiceAreaBySlugQuery request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(s => s.Slug == request.Slug, enableTracking: false, cancellationToken: cancellationToken);
        return entity is null
            ? Result.NotFound<ServiceAreaResponse>($"Service area '{request.Slug}' was not found.")
            : Result.Success(ServiceAreaResponse.FromDomain(entity), "Service area retrieved successfully.");
    }
}
