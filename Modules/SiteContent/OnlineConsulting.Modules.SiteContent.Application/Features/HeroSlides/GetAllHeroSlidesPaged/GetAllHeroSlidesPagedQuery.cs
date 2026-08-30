using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.HeroSlides.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.HeroSlides.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.HeroSlides.GetAllHeroSlidesPaged;

public record GetAllHeroSlidesPagedQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null) : IRequest<OperationDataResult<Paginate<HeroSlideResponse>>>;

public class GetAllHeroSlidesPagedHandler(IHeroSlideRepository repository)
    : IRequestHandler<GetAllHeroSlidesPagedQuery, OperationDataResult<Paginate<HeroSlideResponse>>>
{
    public async Task<OperationDataResult<Paginate<HeroSlideResponse>>> Handle(GetAllHeroSlidesPagedQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request.PageRequest, request.DynamicQuery, defaultOrderBy: x => x.DisplayOrder, tieBreaker: x => x.Id, cancellationToken);

        var response = new Paginate<HeroSlideResponse>
        {
            Items = [.. paged.Items.Select(HeroSlideResponse.FromDomain)],
            Index = paged.Index,
            Size = paged.Size,
            Count = paged.Count,
            Pages = paged.Pages,
        };

        return Result.Success(response, "Hero slides retrieved successfully.");
    }
}
