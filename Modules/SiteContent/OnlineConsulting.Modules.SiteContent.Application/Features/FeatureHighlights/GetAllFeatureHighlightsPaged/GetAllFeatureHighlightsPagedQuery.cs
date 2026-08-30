using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlights.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlights.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlights.GetAllFeatureHighlightsPaged;

public record GetAllFeatureHighlightsPagedQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null) : IRequest<OperationDataResult<Paginate<FeatureHighlightResponse>>>;

public class GetAllFeatureHighlightsPagedHandler(IFeatureHighlightRepository repository)
    : IRequestHandler<GetAllFeatureHighlightsPagedQuery, OperationDataResult<Paginate<FeatureHighlightResponse>>>
{
    public async Task<OperationDataResult<Paginate<FeatureHighlightResponse>>> Handle(GetAllFeatureHighlightsPagedQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request.PageRequest, request.DynamicQuery, defaultOrderBy: x => x.DisplayOrder, tieBreaker: x => x.Id, cancellationToken);

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
