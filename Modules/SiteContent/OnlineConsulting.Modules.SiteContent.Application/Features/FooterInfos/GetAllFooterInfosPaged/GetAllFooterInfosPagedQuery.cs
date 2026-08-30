using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.FooterInfos.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.FooterInfos.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FooterInfos.GetAllFooterInfosPaged;

public record GetAllFooterInfosPagedQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null) : IRequest<OperationDataResult<Paginate<FooterInfoResponse>>>;

public class GetAllFooterInfosPagedHandler(IFooterInfoRepository repository)
    : IRequestHandler<GetAllFooterInfosPagedQuery, OperationDataResult<Paginate<FooterInfoResponse>>>
{
    public async Task<OperationDataResult<Paginate<FooterInfoResponse>>> Handle(GetAllFooterInfosPagedQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request.PageRequest, request.DynamicQuery, defaultOrderBy: x => x.DisplayOrder, tieBreaker: x => x.Id, cancellationToken);

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
