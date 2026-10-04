using MediatR;
using OnlineConsulting.Modules.Services.Application.Features.ServiceMediaItems.Abstractions;
using OnlineConsulting.Modules.Services.Application.Features.ServiceMediaItems.Contracts;
using OnlineConsulting.Modules.Services.Application.Features.Services.Abstractions;
using OnlineConsulting.Modules.Services.Application.Features.Services.Constants;
using OnlineConsulting.Modules.Services.Application.Features.Services.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Services.Application.Features.Services.GetServiceById;

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

        var mediaItems = await mediaItemRepository.GetAllAsync(m => m.ServiceId == service.Id, orderBy: q => q.OrderBy(m => m.DisplayOrder), cancellationToken: cancellationToken);

        return Result.Success(ServiceResponse.FromDomain(service, [.. mediaItems.Select(ServiceMediaItemResponse.FromDomain)]), "Service retrieved successfully.");
    }
}
