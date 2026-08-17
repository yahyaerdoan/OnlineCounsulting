using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceOfferings.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceOfferings.Abstractions;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.ServiceOfferings.GetAllServiceOfferings;

/// <summary>Public - no login required, matches GetAllTestimonialsQuery.</summary>
public record GetAllServiceOfferingsQuery : IRequest<OperationDataResult<List<ServiceOfferingResponse>>>;

public class GetAllServiceOfferingsHandler(IServiceOfferingRepository repository)
    : IRequestHandler<GetAllServiceOfferingsQuery, OperationDataResult<List<ServiceOfferingResponse>>>
{
    public async Task<OperationDataResult<List<ServiceOfferingResponse>>> Handle(GetAllServiceOfferingsQuery request, CancellationToken cancellationToken)
    {
        var entities = await repository.GetListAsync(orderBy: q => q.OrderBy(x => x.DisplayOrder), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        var response = entities.Items.Select(ServiceOfferingResponse.FromDomain).ToList();

        return Result.Success(response, "Service offerings retrieved successfully.");
    }
}
