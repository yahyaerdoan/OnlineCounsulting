using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Services.Application.Contracts;
using OnlineConsulting.Modules.Services.Application.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Services.Application.Features.GetServices;

public record GetServicesQuery(PageRequest PageRequest) : IRequest<OperationDataResult<Paginate<ServiceResponse>>>;

public class GetServicesHandler(IServiceRepository repository)
    : IRequestHandler<GetServicesQuery, OperationDataResult<Paginate<ServiceResponse>>>
{
    public async Task<OperationDataResult<Paginate<ServiceResponse>>> Handle(GetServicesQuery request, CancellationToken cancellationToken)
    {
        var services = await repository.GetListAsync(index: request.PageRequest.PageIndex, size: request.PageRequest.PageSize, cancellationToken: cancellationToken);

        var response = new Paginate<ServiceResponse>
        {
            Items = [.. services.Items.Select(s => ServiceResponse.FromDomain(s))],
            Index = services.Index,
            Size = services.Size,
            Count = services.Count,
            Pages = services.Pages,
        };

        return Result.Success(response, "Services retrieved successfully.");
    }
}
