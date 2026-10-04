using Core.ApplicationLayer.Requests.Lists;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlightsIntros.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlightsIntros.Contracts;
using OnlineConsulting.Modules.SiteContent.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlightsIntros.ListFeatureHighlightsIntros;

public record ListFeatureHighlightsIntrosQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null) : IRequest<OperationDataResult<Paginate<FeatureHighlightsIntroResponse>>>, IDynamicListRequest
{
    public static IReadOnlySet<string> QueryableFields { get; } = new HashSet<string>([nameof(FeatureHighlightsIntro.Description)]);
}

public class ListFeatureHighlightsIntrosHandler(IFeatureHighlightsIntroRepository repository) : IRequestHandler<ListFeatureHighlightsIntrosQuery, OperationDataResult<Paginate<FeatureHighlightsIntroResponse>>>
{
    public async Task<OperationDataResult<Paginate<FeatureHighlightsIntroResponse>>> Handle(ListFeatureHighlightsIntrosQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request, defaultOrderBy: x => x.DisplayOrder, tieBreaker: x => x.Id, cancellationToken: cancellationToken);

        var response = new Paginate<FeatureHighlightsIntroResponse>
        {
            Items = [.. paged.Items.Select(FeatureHighlightsIntroResponse.FromDomain)],
            Index = paged.Index,
            Size = paged.Size,
            Count = paged.Count,
            Pages = paged.Pages,
        };

        return Result.Success(response, "Feature highlights intro entries retrieved successfully.");
    }
}
