using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.Promotions.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.Promotions.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.Promotions.GetAllPromotions;

public record GetAllPromotionsQuery : IRequest<OperationDataResult<List<PromotionResponse>>>;

public class GetAllPromotionsHandler(IPromotionRepository repository) : IRequestHandler<GetAllPromotionsQuery, OperationDataResult<List<PromotionResponse>>>
{
    public async Task<OperationDataResult<List<PromotionResponse>>> Handle(GetAllPromotionsQuery request, CancellationToken cancellationToken)
    {
        var entities = await repository.GetAllAsync(orderBy: q => q.OrderBy(x => x.DisplayOrder), cancellationToken: cancellationToken);
        var response = entities.Select(PromotionResponse.FromDomain).ToList();

        return Result.Success(response, "Promotions retrieved successfully.");
    }
}
