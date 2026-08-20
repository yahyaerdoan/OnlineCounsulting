using MediatR;
using OnlineConsulting.Modules.Services.Application.Abstractions;
using OnlineConsulting.Modules.Services.Application.Contracts;
using OnlineConsulting.Modules.Services.Application.Features.Constants;
using OnlineConsulting.Modules.Services.Application.Features.ServiceMediaItems.Abstractions;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Services.Application.Features.GetServiceById;

public record GetServiceByIdQuery(Guid Id) : IRequest<OperationDataResult<ServiceResponse>>;

public class GetServiceByIdHandler(IServiceRepository repository, IServiceMediaItemRepository mediaItemRepository)
    : IRequestHandler<GetServiceByIdQuery, OperationDataResult<ServiceResponse>>
{
    public async Task<OperationDataResult<ServiceResponse>> Handle(GetServiceByIdQuery request, CancellationToken cancellationToken)
    {
        var service = await repository.GetAsync(s => s.Id == request.Id, enableTracking: false, cancellationToken: cancellationToken);
        if (service is null)
        {
            return Result.NotFound<ServiceResponse>(string.Format(ServiceMessages.ServiceNotFoundFormat, request.Id));
        }

        var mediaItems = await mediaItemRepository.GetListAsync(
            m => m.ServiceId == service.Id, orderBy: q => q.OrderBy(m => m.DisplayOrder),
            size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);

        return Result.Success(ServiceResponse.FromDomain(service, [.. mediaItems.Items.Select(ServiceMediaItemResponse.FromDomain)]), "Service retrieved successfully.");
    }
}
