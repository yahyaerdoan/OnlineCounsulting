using MediatR;
using OnlineConsulting.Modules.Services.Application.Features.Services.Abstractions;
using OnlineConsulting.Modules.Services.Application.Features.Services.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Services.Application.Features.Services.SearchServices;

public record SearchServicesQuery(string Query) : IRequest<OperationDataResult<List<ServiceResponse>>>;

public class SearchServicesHandler(IServiceRepository repository) : IRequestHandler<SearchServicesQuery, OperationDataResult<List<ServiceResponse>>>
{
    public async Task<OperationDataResult<List<ServiceResponse>>> Handle(SearchServicesQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
        {
            return Result.Success(new List<ServiceResponse>(), "No search query provided.");
        }

        var services = await repository.GetAllAsync(s => s.Title.Contains(request.Query) || s.Description.Contains(request.Query), cancellationToken: cancellationToken);

        List<ServiceResponse> response = [.. services.Select(s => ServiceResponse.FromDomain(s))];

        return Result.Success(response, response.Count == 0 ? "No services matched the search query." : "Services retrieved successfully.");
    }
}
