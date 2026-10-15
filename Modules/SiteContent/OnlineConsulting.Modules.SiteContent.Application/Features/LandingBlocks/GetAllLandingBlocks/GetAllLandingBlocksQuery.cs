using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.LandingBlocks.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.LandingBlocks.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.LandingBlocks.GetAllLandingBlocks;

/// <summary>Public; every section's blocks in display order, grouped by the caller.</summary>
public record GetAllLandingBlocksQuery : IRequest<OperationDataResult<List<LandingBlockResponse>>>;

public class GetAllLandingBlocksHandler(ILandingBlockRepository repository)
    : IRequestHandler<GetAllLandingBlocksQuery, OperationDataResult<List<LandingBlockResponse>>>
{
    public async Task<OperationDataResult<List<LandingBlockResponse>>> Handle(GetAllLandingBlocksQuery request, CancellationToken cancellationToken)
    {
        var entities = await repository.GetAllAsync(orderBy: q => q.OrderBy(x => x.DisplayOrder), enableTracking: false, cancellationToken: cancellationToken);
        var response = entities.Select(LandingBlockResponse.FromDomain).ToList();

        return Result.Success(response, "Landing blocks retrieved successfully.");
    }
}
