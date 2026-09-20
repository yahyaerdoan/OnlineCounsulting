using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlightsIntros.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlightsIntros.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlightsIntros.GetAllFeatureHighlightsIntrosPaged;

public record GetAllFeatureHighlightsIntrosPagedQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null) : IRequest<OperationDataResult<Paginate<FeatureHighlightsIntroResponse>>>;

public class GetAllFeatureHighlightsIntrosPagedHandler(IFeatureHighlightsIntroRepository repository) : IRequestHandler<GetAllFeatureHighlightsIntrosPagedQuery, OperationDataResult<Paginate<FeatureHighlightsIntroResponse>>>
{
    public async Task<OperationDataResult<Paginate<FeatureHighlightsIntroResponse>>> Handle(GetAllFeatureHighlightsIntrosPagedQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request.PageRequest, request.DynamicQuery, defaultOrderBy: x => x.DisplayOrder, tieBreaker: x => x.Id, cancellationToken);

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
