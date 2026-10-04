using Core.ApplicationLayer.Requests.Lists;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlights.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlights.Contracts;
using OnlineConsulting.Modules.SiteContent.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlights.ListFeatureHighlights;

public record ListFeatureHighlightsQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null) : IRequest<OperationDataResult<Paginate<FeatureHighlightResponse>>>, IDynamicListRequest
{
    public static IReadOnlySet<string> QueryableFields { get; } = new HashSet<string>([nameof(FeatureHighlight.Title), nameof(FeatureHighlight.Description)]);
}

public class ListFeatureHighlightsHandler(IFeatureHighlightRepository repository)
    : IRequestHandler<ListFeatureHighlightsQuery, OperationDataResult<Paginate<FeatureHighlightResponse>>>
{
    public async Task<OperationDataResult<Paginate<FeatureHighlightResponse>>> Handle(ListFeatureHighlightsQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request, defaultOrderBy: x => x.DisplayOrder, tieBreaker: x => x.Id, cancellationToken: cancellationToken);

        var response = new Paginate<FeatureHighlightResponse>
        {
            Items = [.. paged.Items.Select(FeatureHighlightResponse.FromDomain)],
            Index = paged.Index,
            Size = paged.Size,
            Count = paged.Count,
            Pages = paged.Pages,
        };

        return Result.Success(response, "Feature highlights retrieved successfully.");
    }
}
