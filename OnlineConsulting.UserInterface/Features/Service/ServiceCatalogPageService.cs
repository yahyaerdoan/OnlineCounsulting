using OnlineConsulting.UserInterface.Features.Category;
using OnlineConsulting.UserInterface.Infrastructure.Media;

namespace OnlineConsulting.UserInterface.Features.Service;

public class ServiceCatalogPageService(IServiceCatalogService serviceCatalogService, ICategoryService categoryService, IMediaService mediaService) : IServiceCatalogPageService
{
    public async Task<ServiceListViewModel> GetPagedAsync(int page, int size, CancellationToken cancellationToken = default)
    {
        var all = await serviceCatalogService.GetAllAsync(cancellationToken: cancellationToken);
        var categoryTitles = await GetCategoryTitlesAsync(cancellationToken);

        var pageItems = all.Skip((page - 1) * size).Take(size);
        var cards = new List<ServiceCardViewModel>();
        foreach (var service in pageItems)
        {
            cards.Add(await ToCardAsync(service, categoryTitles, cancellationToken));
        }

        return new ServiceListViewModel(cards, page, size, all.Count);
    }

    public async Task<ServiceDetailViewModel?> GetDetailAsync(string slug, CancellationToken cancellationToken = default)
    {
        var service = await serviceCatalogService.GetBySlugAsync(slug, cancellationToken);
        if (service is null)
        {
            return null;
        }

        var category = await categoryService.GetByIdAsync(service.CategoryId, cancellationToken);

        var imageUrls = new List<string>();
        var coverUrl = await mediaService.ResolveUrlAsync(service.CoverMediaAssetId, cancellationToken);
        if (coverUrl is not null)
        {
            imageUrls.Add(coverUrl);
        }

        foreach (var mediaItem in service.MediaItems.OrderBy(m => m.DisplayOrder))
        {
            var url = await mediaService.ResolveUrlAsync(mediaItem.MediaAssetId, cancellationToken);
            if (url is not null)
            {
                imageUrls.Add(url);
            }
        }

        return new ServiceDetailViewModel(service.Id, service.Title, service.Slug, service.Description, service.DetailedDescription,
            service.CategoryId, category?.Title ?? string.Empty, service.Price, service.DiscountedPrice, service.DiscountRate, imageUrls);
    }

    public async Task<List<ServiceCardViewModel>> GetRelatedAsync(string slug, CancellationToken cancellationToken = default)
    {
        var service = await serviceCatalogService.GetBySlugAsync(slug, cancellationToken);
        if (service is null)
        {
            return [];
        }

        var categoryServices = await serviceCatalogService.GetByCategoryAsync(service.CategoryId, cancellationToken: cancellationToken);
        var categoryTitles = await GetCategoryTitlesAsync(cancellationToken);

        var cards = new List<ServiceCardViewModel>();
        foreach (var related in categoryServices.Where(s => s.Id != service.Id))
        {
            cards.Add(await ToCardAsync(related, categoryTitles, cancellationToken));
        }

        return cards;
    }

    private async Task<Dictionary<Guid, string>> GetCategoryTitlesAsync(CancellationToken cancellationToken) =>
        (await categoryService.GetAllAsync(cancellationToken: cancellationToken)).ToDictionary(c => c.Id, c => c.Title);

    private async Task<ServiceCardViewModel> ToCardAsync(ServiceCatalogResponse service, Dictionary<Guid, string> categoryTitles, CancellationToken cancellationToken)
    {
        var coverImageUrl = await mediaService.ResolveUrlAsync(service.CoverMediaAssetId, cancellationToken);
        return new ServiceCardViewModel(service.Id, service.Title, service.Slug, service.Description,
            categoryTitles.GetValueOrDefault(service.CategoryId, string.Empty), service.Price, service.DiscountedPrice, service.DiscountRate, coverImageUrl);
    }
}
