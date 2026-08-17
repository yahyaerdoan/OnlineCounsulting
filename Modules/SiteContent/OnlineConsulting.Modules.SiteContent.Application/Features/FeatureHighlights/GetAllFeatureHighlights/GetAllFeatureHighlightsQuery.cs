using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlights.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlights.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlights.GetAllFeatureHighlights;

public record GetAllFeatureHighlightsQuery : IRequest<OperationDataResult<List<FeatureHighlightResponse>>>;

public class GetAllFeatureHighlightsHandler(IFeatureHighlightRepository repository) : IRequestHandler<GetAllFeatureHighlightsQuery, OperationDataResult<List<FeatureHighlightResponse>>>
{
    public async Task<OperationDataResult<List<FeatureHighlightResponse>>> Handle(GetAllFeatureHighlightsQuery request, CancellationToken cancellationToken)
    {
        var entities = await repository.GetListAsync(orderBy: q => q.OrderBy(x => x.DisplayOrder), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        var response = entities.Items.Select(FeatureHighlightResponse.FromDomain).ToList();

        return Result.Success(response, "Feature highlights retrieved successfully.");
    }
}
