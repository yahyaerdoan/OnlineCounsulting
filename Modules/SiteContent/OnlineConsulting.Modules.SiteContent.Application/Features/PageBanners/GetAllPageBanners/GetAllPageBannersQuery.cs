using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.PageBanners.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.PageBanners.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.PageBanners.GetAllPageBanners;

public record GetAllPageBannersQuery : IRequest<OperationDataResult<List<PageBannerResponse>>>;

public class GetAllPageBannersHandler(IPageBannerRepository repository) : IRequestHandler<GetAllPageBannersQuery, OperationDataResult<List<PageBannerResponse>>>
{
    public async Task<OperationDataResult<List<PageBannerResponse>>> Handle(GetAllPageBannersQuery request, CancellationToken cancellationToken)
    {
        var entities = await repository.GetAllAsync(orderBy: q => q.OrderBy(x => x.DisplayOrder), cancellationToken: cancellationToken);
        var response = entities.Select(PageBannerResponse.FromDomain).ToList();

        return Result.Success(response, "Page banners retrieved successfully.");
    }
}
