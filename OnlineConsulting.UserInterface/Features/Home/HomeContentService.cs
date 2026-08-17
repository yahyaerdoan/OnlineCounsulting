using OnlineConsulting.UserInterface.Features.Category;
using OnlineConsulting.UserInterface.Features.Service;
using OnlineConsulting.UserInterface.Infrastructure.Media;

namespace OnlineConsulting.UserInterface.Features.Home;

public class HomeContentService(ICategoryService categoryService, IServiceCatalogService serviceCatalogService, IMediaService mediaService) : IHomeContentService
{
    public Task<List<CategoryResponse>> GetCategoriesAsync(CancellationToken cancellationToken = default) =>
        categoryService.GetAllAsync(cancellationToken: cancellationToken);

    public async Task<List<HomeFeaturedServiceViewModel>> GetFeaturedServicesAsync(CancellationToken cancellationToken = default)
    {
        var services = await serviceCatalogService.GetFeaturedAsync(cancellationToken);
        var categoryTitles = (await categoryService.GetAllAsync(cancellationToken: cancellationToken)).ToDictionary(c => c.Id, c => c.Title);

        var items = new List<HomeFeaturedServiceViewModel>();
        foreach (var service in services)
        {
            var coverImageUrl = await mediaService.ResolveUrlAsync(service.CoverMediaAssetId, cancellationToken);
            items.Add(new HomeFeaturedServiceViewModel(
                service.Id, service.Title, service.Slug, service.Description,
                categoryTitles.GetValueOrDefault(service.CategoryId, string.Empty),
                service.Price, service.DiscountedPrice, service.DiscountRate, coverImageUrl));
        }

        return items;
    }
}
