using MediatR;
using OnlineConsulting.Modules.Services.Application.Features.ServiceMediaItems.Abstractions;
using OnlineConsulting.Modules.Services.Application.Features.ServiceMediaItems.Contracts;
using OnlineConsulting.Modules.Services.Application.Features.Services.Abstractions;
using OnlineConsulting.Modules.Services.Application.Features.Services.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Services.Application.Features.Services.GetServiceBySlug;

public record GetServiceBySlugQuery(string Slug) : IRequest<OperationDataResult<ServiceResponse>>;

public class GetServiceBySlugHandler(IServiceRepository repository, IServiceMediaItemRepository mediaItemRepository)
    : IRequestHandler<GetServiceBySlugQuery, OperationDataResult<ServiceResponse>>
{
    public async Task<OperationDataResult<ServiceResponse>> Handle(GetServiceBySlugQuery request, CancellationToken cancellationToken)
    {
        var service = await repository.GetAsync(s => s.Slug == request.Slug, enableTracking: false, cancellationToken: cancellationToken);
        if (service is null)
        {
            return Result.NotFound<ServiceResponse>($"Service '{request.Slug}' was not found.");
        }

        var mediaItems = await mediaItemRepository.GetAllAsync(m => m.ServiceId == service.Id, orderBy: q => q.OrderBy(m => m.DisplayOrder), cancellationToken: cancellationToken);

        return Result.Success(ServiceResponse.FromDomain(service, [.. mediaItems.Select(ServiceMediaItemResponse.FromDomain)]), "Service retrieved successfully.");
    }
}
