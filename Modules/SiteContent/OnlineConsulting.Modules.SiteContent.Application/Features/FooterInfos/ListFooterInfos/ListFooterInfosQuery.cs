using Core.ApplicationLayer.Requests.Lists;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.FooterInfos.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.FooterInfos.Contracts;
using OnlineConsulting.Modules.SiteContent.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FooterInfos.ListFooterInfos;

public record ListFooterInfosQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null) : IRequest<OperationDataResult<Paginate<FooterInfoResponse>>>, IDynamicListRequest
{
    public static IReadOnlySet<string> QueryableFields { get; } = new HashSet<string>([nameof(FooterInfo.Id), nameof(FooterInfo.Description), nameof(FooterInfo.DisplayOrder)]);
}

public class ListFooterInfosHandler(IFooterInfoRepository repository)
    : IRequestHandler<ListFooterInfosQuery, OperationDataResult<Paginate<FooterInfoResponse>>>
{
    public async Task<OperationDataResult<Paginate<FooterInfoResponse>>> Handle(ListFooterInfosQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request, defaultOrderBy: x => x.DisplayOrder, tieBreaker: x => x.Id, cancellationToken: cancellationToken);

        var response = new Paginate<FooterInfoResponse>
        {
            Items = [.. paged.Items.Select(FooterInfoResponse.FromDomain)],
            Index = paged.Index,
            Size = paged.Size,
            Count = paged.Count,
            Pages = paged.Pages,
        };

        return Result.Success(response, "Footer info entries retrieved successfully.");
    }
}
