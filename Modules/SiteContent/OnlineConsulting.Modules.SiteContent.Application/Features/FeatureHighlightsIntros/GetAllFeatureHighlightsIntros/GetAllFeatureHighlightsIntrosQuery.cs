using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlightsIntros.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlightsIntros.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlightsIntros.GetAllFeatureHighlightsIntros;

public record GetAllFeatureHighlightsIntrosQuery : IRequest<OperationDataResult<List<FeatureHighlightsIntroResponse>>>;

public class GetAllFeatureHighlightsIntrosHandler(IFeatureHighlightsIntroRepository repository) : IRequestHandler<GetAllFeatureHighlightsIntrosQuery, OperationDataResult<List<FeatureHighlightsIntroResponse>>>
{
    public async Task<OperationDataResult<List<FeatureHighlightsIntroResponse>>> Handle(GetAllFeatureHighlightsIntrosQuery request, CancellationToken cancellationToken)
    {
        var entities = await repository.GetAllAsync(orderBy: q => q.OrderBy(x => x.DisplayOrder), cancellationToken: cancellationToken);
        var response = entities.Select(FeatureHighlightsIntroResponse.FromDomain).ToList();

        return Result.Success(response, "Feature highlights intro retrieved successfully.");
    }
}
