using Core.ApplicationLayer.Requests.Lists;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.LandingBlocks.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.LandingBlocks.Contracts;
using OnlineConsulting.Modules.SiteContent.Domain.Landing;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.LandingBlocks.ListLandingBlocks;

/// <summary>Sortable/filterable variant of GetAllLandingBlocksQuery for the admin ServerDataTable.</summary>
public record ListLandingBlocksQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null) : IRequest<OperationDataResult<Paginate<LandingBlockResponse>>>, IDynamicListRequest
{
    public static IReadOnlySet<string> QueryableFields { get; } = new HashSet<string>([nameof(LandingBlock.Section), nameof(LandingBlock.Title), nameof(LandingBlock.Body), nameof(LandingBlock.DisplayOrder)]);
}

public class ListLandingBlocksHandler(ILandingBlockRepository repository)
    : IRequestHandler<ListLandingBlocksQuery, OperationDataResult<Paginate<LandingBlockResponse>>>
{
    public async Task<OperationDataResult<Paginate<LandingBlockResponse>>> Handle(ListLandingBlocksQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request, defaultOrderBy: x => x.DisplayOrder, tieBreaker: x => x.Id, cancellationToken: cancellationToken);

        var response = new Paginate<LandingBlockResponse>
        {
            Items = [.. paged.Items.Select(LandingBlockResponse.FromDomain)],
            Index = paged.Index,
            Size = paged.Size,
            Count = paged.Count,
            Pages = paged.Pages,
        };

        return Result.Success(response, "Landing blocks retrieved successfully.");
    }
}
