using MediatR;
using OnlineConsulting.Modules.Services.Application.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Services.Application.Features.GetFeaturedServices;

public record GetFeaturedServicesQuery : IRequest<OperationDataResult<List<ServiceResponse>>>;

public class GetFeaturedServicesHandler(IServiceRepository repository) : IRequestHandler<GetFeaturedServicesQuery, OperationDataResult<List<ServiceResponse>>>
{
    public async Task<OperationDataResult<List<ServiceResponse>>> Handle(GetFeaturedServicesQuery request, CancellationToken cancellationToken)
    {
        var services = await repository.GetListAsync(s => s.FeaturedArea, size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);

        List<ServiceResponse> response = [.. services.Items.Select(ServiceResponse.FromDomain)];

        return Result.Success(response, "Featured services retrieved successfully.");
    }
}
