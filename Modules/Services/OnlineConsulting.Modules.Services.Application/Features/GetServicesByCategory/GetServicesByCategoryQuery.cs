using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Services.Application.Abstractions;
using OnlineConsulting.Modules.Services.Application.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Services.Application.Features.GetServicesByCategory;

public record GetServicesByCategoryQuery(Guid CategoryId, PageRequest PageRequest) : IRequest<OperationDataResult<Paginate<ServiceResponse>>>;

public class GetServicesByCategoryHandler(IServiceRepository repository)
    : IRequestHandler<GetServicesByCategoryQuery, OperationDataResult<Paginate<ServiceResponse>>>
{
    public async Task<OperationDataResult<Paginate<ServiceResponse>>> Handle(GetServicesByCategoryQuery request, CancellationToken cancellationToken)
    {
        var services = await repository.GetListAsync(s => s.CategoryId == request.CategoryId,
            index: request.PageRequest.PageIndex, size: request.PageRequest.PageSize, cancellationToken: cancellationToken);

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
