using MediatR;
using OnlineConsulting.Modules.Services.Application.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Services.Application.Features.GetServiceBySlug;

public record GetServiceBySlugQuery(string Slug) : IRequest<OperationDataResult<ServiceResponse>>;

public class GetServiceBySlugHandler(IServiceRepository repository) : IRequestHandler<GetServiceBySlugQuery, OperationDataResult<ServiceResponse>>
{
    public async Task<OperationDataResult<ServiceResponse>> Handle(GetServiceBySlugQuery request, CancellationToken cancellationToken)
    {
        var service = await repository.GetAsync(s => s.Slug == request.Slug, enableTracking: false, cancellationToken: cancellationToken);

        return service is null
            ? Result.NotFound<ServiceResponse>($"Service '{request.Slug}' was not found.")
            : Result.Success(ServiceResponse.FromDomain(service), "Service retrieved successfully.");
    }
}
