using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.PageBanners.GetAllPageBanners;

public record GetAllPageBannersQuery : IRequest<OperationDataResult<List<PageBannerResponse>>>;

public class GetAllPageBannersHandler(IPageBannerRepository repository) : IRequestHandler<GetAllPageBannersQuery, OperationDataResult<List<PageBannerResponse>>>
{
    public async Task<OperationDataResult<List<PageBannerResponse>>> Handle(GetAllPageBannersQuery request, CancellationToken cancellationToken)
    {
        var entities = await repository.GetListAsync(orderBy: q => q.OrderBy(x => x.DisplayOrder), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        var response = entities.Items.Select(PageBannerResponse.FromDomain).ToList();

        return Result.Success(response, "Page banners retrieved successfully.");
    }
}
