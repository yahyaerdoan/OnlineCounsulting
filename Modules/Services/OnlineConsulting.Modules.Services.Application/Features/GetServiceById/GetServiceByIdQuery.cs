using MediatR;
using OnlineConsulting.Modules.Services.Application.Contracts;
using OnlineConsulting.Modules.Services.Application.Features.Constants;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Services.Application.Features.GetServiceById;

public record GetServiceByIdQuery(Guid Id) : IRequest<OperationDataResult<ServiceResponse>>;

public class GetServiceByIdHandler(IServiceRepository repository) : IRequestHandler<GetServiceByIdQuery, OperationDataResult<ServiceResponse>>
{
    public async Task<OperationDataResult<ServiceResponse>> Handle(GetServiceByIdQuery request, CancellationToken cancellationToken)
    {
        var service = await repository.GetAsync(s => s.Id == request.Id, enableTracking: false, cancellationToken: cancellationToken);

        return service is null
            ? Result.NotFound<ServiceResponse>(string.Format(ServiceMessages.ServiceNotFoundFormat, request.Id))
            : Result.Success(ServiceResponse.FromDomain(service), "Service retrieved successfully.");
    }
}
