using MediatR;
using OnlineConsulting.Modules.Services.Application.Features.Services.Abstractions;
using OnlineConsulting.Modules.Services.Application.Features.Services.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Services.Application.Features.Services.GetFeaturedServices;

public record GetFeaturedServicesQuery : IRequest<OperationDataResult<List<ServiceResponse>>>;

public class GetFeaturedServicesHandler(IServiceRepository repository) : IRequestHandler<GetFeaturedServicesQuery, OperationDataResult<List<ServiceResponse>>>
{
    public async Task<OperationDataResult<List<ServiceResponse>>> Handle(GetFeaturedServicesQuery request, CancellationToken cancellationToken)
    {
        var services = await repository.GetAllAsync(s => s.FeaturedArea, cancellationToken: cancellationToken);

        List<ServiceResponse> response = [.. services.Select(s => ServiceResponse.FromDomain(s))];

        return Result.Success(response, "Featured services retrieved successfully.");
    }
}
