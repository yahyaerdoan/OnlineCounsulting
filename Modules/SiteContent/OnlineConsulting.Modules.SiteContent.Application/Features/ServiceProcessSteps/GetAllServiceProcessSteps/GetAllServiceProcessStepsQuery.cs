using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.ServiceProcessSteps.GetAllServiceProcessSteps;

/// <summary>Public - no login required, matches GetAllTestimonialsQuery.</summary>
public record GetAllServiceProcessStepsQuery : IRequest<OperationDataResult<List<ServiceProcessStepResponse>>>;

public class GetAllServiceProcessStepsHandler(IServiceProcessStepRepository repository)
    : IRequestHandler<GetAllServiceProcessStepsQuery, OperationDataResult<List<ServiceProcessStepResponse>>>
{
    public async Task<OperationDataResult<List<ServiceProcessStepResponse>>> Handle(GetAllServiceProcessStepsQuery request, CancellationToken cancellationToken)
    {
        var entities = await repository.GetListAsync(orderBy: q => q.OrderBy(x => x.DisplayOrder), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        var response = entities.Items.Select(ServiceProcessStepResponse.FromDomain).ToList();

        return Result.Success(response, "Service process steps retrieved successfully.");
    }
}
