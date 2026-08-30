using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.FaqItems.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.FaqItems.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FaqItems.GetAllFaqItemsPaged;

public record GetAllFaqItemsPagedQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null) : IRequest<OperationDataResult<Paginate<FaqItemResponse>>>;

public class GetAllFaqItemsPagedHandler(IFaqItemRepository repository)
    : IRequestHandler<GetAllFaqItemsPagedQuery, OperationDataResult<Paginate<FaqItemResponse>>>
{
    public async Task<OperationDataResult<Paginate<FaqItemResponse>>> Handle(GetAllFaqItemsPagedQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request.PageRequest, request.DynamicQuery, defaultOrderBy: x => x.DisplayOrder, tieBreaker: x => x.Id, cancellationToken);

        var response = new Paginate<FaqItemResponse>
        {
            Items = [.. paged.Items.Select(FaqItemResponse.FromDomain)],
            Index = paged.Index,
            Size = paged.Size,
            Count = paged.Count,
            Pages = paged.Pages,
        };

        return Result.Success(response, "FAQ items retrieved successfully.");
    }
}
