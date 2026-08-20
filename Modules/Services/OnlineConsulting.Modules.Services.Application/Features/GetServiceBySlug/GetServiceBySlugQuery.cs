using MediatR;
using OnlineConsulting.Modules.Services.Application.Abstractions;
using OnlineConsulting.Modules.Services.Application.Contracts;
using OnlineConsulting.Modules.Services.Application.Features.ServiceMediaItems.Abstractions;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Services.Application.Features.GetServiceBySlug;

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

        var mediaItems = await mediaItemRepository.GetListAsync(
            m => m.ServiceId == service.Id, orderBy: q => q.OrderBy(m => m.DisplayOrder),
            size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);

        return Result.Success(ServiceResponse.FromDomain(service, [.. mediaItems.Items.Select(ServiceMediaItemResponse.FromDomain)]), "Service retrieved successfully.");
    }
}
