using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.Promotions.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.Promotions.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.Promotions.GetAllPromotionsPaged;

public record GetAllPromotionsPagedQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null) : IRequest<OperationDataResult<Paginate<PromotionResponse>>>;

public class GetAllPromotionsPagedHandler(IPromotionRepository repository)
    : IRequestHandler<GetAllPromotionsPagedQuery, OperationDataResult<Paginate<PromotionResponse>>>
{
    public async Task<OperationDataResult<Paginate<PromotionResponse>>> Handle(GetAllPromotionsPagedQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request.PageRequest, request.DynamicQuery, defaultOrderBy: x => x.DisplayOrder, tieBreaker: x => x.Id, cancellationToken);

        var response = new Paginate<PromotionResponse>
        {
            Items = [.. paged.Items.Select(PromotionResponse.FromDomain)],
            Index = paged.Index,
            Size = paged.Size,
            Count = paged.Count,
            Pages = paged.Pages,
        };

        return Result.Success(response, "Promotions retrieved successfully.");
    }
}
