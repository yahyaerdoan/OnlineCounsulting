using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.Promotions.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.Promotions.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.Promotions.GetAllPromotions;

public record GetAllPromotionsQuery : IRequest<OperationDataResult<List<PromotionResponse>>>;

public class GetAllPromotionsHandler(IPromotionRepository repository) : IRequestHandler<GetAllPromotionsQuery, OperationDataResult<List<PromotionResponse>>>
{
    public async Task<OperationDataResult<List<PromotionResponse>>> Handle(GetAllPromotionsQuery request, CancellationToken cancellationToken)
    {
        var entities = await repository.GetListAsync(orderBy: q => q.OrderBy(x => x.DisplayOrder), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        var response = entities.Items.Select(PromotionResponse.FromDomain).ToList();

        return Result.Success(response, "Promotions retrieved successfully.");
    }
}
