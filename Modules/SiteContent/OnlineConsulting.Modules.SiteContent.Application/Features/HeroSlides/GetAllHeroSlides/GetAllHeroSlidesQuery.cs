using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.HeroSlides.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.HeroSlides.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.HeroSlides.GetAllHeroSlides;

public record GetAllHeroSlidesQuery : IRequest<OperationDataResult<List<HeroSlideResponse>>>;

public class GetAllHeroSlidesHandler(IHeroSlideRepository repository) : IRequestHandler<GetAllHeroSlidesQuery, OperationDataResult<List<HeroSlideResponse>>>
{
    public async Task<OperationDataResult<List<HeroSlideResponse>>> Handle(GetAllHeroSlidesQuery request, CancellationToken cancellationToken)
    {
        var entities = await repository.GetListAsync(orderBy: q => q.OrderBy(x => x.DisplayOrder), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        var response = entities.Items.Select(HeroSlideResponse.FromDomain).ToList();

        return Result.Success(response, "Hero slides retrieved successfully.");
    }
}
