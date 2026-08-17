using OnlineConsulting.UserInterface.Infrastructure.Api;
using OnlineConsulting.UserInterface.Features.Category;
using OnlineConsulting.UserInterface.Features.Service;
using OnlineConsulting.UserInterface.Infrastructure.Media;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Service;

public class AdminServiceCatalogService(
    IServiceCatalogService serviceCatalogService,
    ICategoryService categoryService,
    IMediaService mediaService) : IAdminServiceCatalogService
{
    /// <summary>The admin list screens have no pagination UI, so everything is pulled as one page.</summary>
    private const int SinglePageSize = 1000;

    public async Task<List<ServiceListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var services = await serviceCatalogService.GetAllAsync(0, SinglePageSize, cancellationToken);
        var categoryTitles = await GetCategoryTitlesAsync(cancellationToken);

        var items = new List<ServiceListItemViewModel>();
        foreach (var service in services)
        {
            items.Add(new ServiceListItemViewModel(
                service.Id,
                service.Title,
                categoryTitles.GetValueOrDefault(service.CategoryId, string.Empty),
                service.Description,
                service.DetailedDescription,
                service.Price,
                service.DiscountRate,
                service.DiscountedPrice,
                service.TaxRate,
                service.FeaturedArea,
                await mediaService.ResolveUrlAsync(service.CoverMediaAssetId, cancellationToken)));
        }

        return items;
    }

    public async Task<CreateServiceViewModel> BuildCreateModelAsync(CancellationToken cancellationToken = default)
    {
        var model = new CreateServiceViewModel();
        await FillCategoriesAsync(model, cancellationToken);
        return model;
    }

    public async Task<UpdateServiceViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var service = await serviceCatalogService.GetByIdAsync(id, cancellationToken);
        if (service is null)
            return null;

        var model = new UpdateServiceViewModel
        {
            Id = service.Id,
            CategoryId = service.CategoryId,
            Title = service.Title,
            Description = service.Description,
            DetailedDescription = service.DetailedDescription,
            Price = service.Price,
            DiscountRate = service.DiscountRate,
            TaxRate = service.TaxRate,
            FeaturedArea = service.FeaturedArea,
            RequiresPrepayment = service.RequiresPrepayment,
            ExistingImages = await ResolveImagesAsync(service, cancellationToken),
        };

        await FillCategoriesAsync(model, cancellationToken);
        return model;
    }

    public async Task FillCategoriesAsync(CreateServiceViewModel model, CancellationToken cancellationToken = default)
    {
        var categories = await categoryService.GetAllAsync(0, SinglePageSize, cancellationToken);
        model.Categories = categories.Select(c => new CategoryOptionViewModel(c.Id, c.Title)).ToList();
    }

    public async Task<ApiEnvelope> CreateAsync(CreateServiceViewModel model, CancellationToken cancellationToken = default)
    {
        // Photos are uploaded before the service exists so the first one can be its cover in a single create call.
        var assetIds = await UploadAllAsync(model.Images, cancellationToken);

        var created = await serviceCatalogService.CreateAsync(
            model.CategoryId, model.Title, model.Description, model.DetailedDescription, model.Price,
            model.FeaturedArea, model.DiscountRate, model.TaxRate, model.RequiresPrepayment,
            assetIds.FirstOrDefault(), cancellationToken);

        if (!created.IsSuccessful || created.ResultData == default)
            return created.WithoutData();

        foreach (var assetId in assetIds.Skip(1))
            await serviceCatalogService.AddMediaItemAsync(created.ResultData, assetId, cancellationToken: cancellationToken);

        return created.WithoutData();
    }

    public async Task<ApiEnvelope> UpdateAsync(UpdateServiceViewModel model, CancellationToken cancellationToken = default)
    {
        var current = await serviceCatalogService.GetByIdAsync(model.Id, cancellationToken);
        var assetIds = await UploadAllAsync(model.Images, cancellationToken);
        var coverMediaAssetId = current?.CoverMediaAssetId ?? assetIds.FirstOrDefault();

        var result = await serviceCatalogService.UpdateAsync(
            model.Id, model.CategoryId, model.Title, model.Description, model.DetailedDescription, model.Price,
            model.FeaturedArea, model.DiscountRate, model.TaxRate, model.RequiresPrepayment,
            coverMediaAssetId, cancellationToken);

        if (!result.IsSuccessful)
            return result;

        foreach (var assetId in assetIds.Where(id => id != coverMediaAssetId))
            await serviceCatalogService.AddMediaItemAsync(model.Id, assetId, cancellationToken: cancellationToken);

        return result;
    }

    public Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        serviceCatalogService.DeleteAsync(id, cancellationToken);

    public async Task<ApiEnvelope> AddImageAsync(Guid serviceId, IFormFile? image, CancellationToken cancellationToken = default)
    {
        var assetId = await mediaService.UploadAsync(image, cancellationToken);
        if (assetId is null)
            return new ApiEnvelope(false, StatusCodes.Status400BadRequest, "No image was selected.", null);

        var service = await serviceCatalogService.GetByIdAsync(serviceId, cancellationToken);
        if (service is null)
            return new ApiEnvelope(false, StatusCodes.Status404NotFound, "Service not found.", null);

        if (service.CoverMediaAssetId is null)
            return await SetCoverAsync(service, assetId, cancellationToken);

        return (await serviceCatalogService.AddMediaItemAsync(serviceId, assetId.Value, cancellationToken: cancellationToken)).WithoutData();
    }

    public async Task<ApiEnvelope> RemoveImageAsync(Guid serviceId, Guid imageId, CancellationToken cancellationToken = default)
    {
        var service = await serviceCatalogService.GetByIdAsync(serviceId, cancellationToken);
        if (service is null)
            return new ApiEnvelope(false, StatusCodes.Status404NotFound, "Service not found.", null);

        return service.CoverMediaAssetId == imageId
            ? await SetCoverAsync(service, null, cancellationToken)
            : await serviceCatalogService.RemoveMediaItemAsync(imageId, cancellationToken);
    }

    private Task<ApiEnvelope> SetCoverAsync(ServiceCatalogResponse service, Guid? coverMediaAssetId, CancellationToken cancellationToken) =>
        serviceCatalogService.UpdateAsync(
            service.Id, service.CategoryId, service.Title, service.Description, service.DetailedDescription,
            service.Price, service.FeaturedArea, service.DiscountRate, service.TaxRate, service.RequiresPrepayment,
            coverMediaAssetId, cancellationToken);

    private async Task<List<Guid>> UploadAllAsync(List<IFormFile>? files, CancellationToken cancellationToken)
    {
        var assetIds = new List<Guid>();
        foreach (var file in files ?? [])
        {
            var assetId = await mediaService.UploadAsync(file, cancellationToken);
            if (assetId is { } id)
                assetIds.Add(id);
        }

        return assetIds;
    }

    private async Task<List<ServiceImageViewModel>> ResolveImagesAsync(ServiceCatalogResponse service, CancellationToken cancellationToken)
    {
        var images = new List<ServiceImageViewModel>();

        var coverUrl = await mediaService.ResolveUrlAsync(service.CoverMediaAssetId, cancellationToken);
        if (coverUrl is not null && service.CoverMediaAssetId is { } coverId)
            images.Add(new ServiceImageViewModel(coverId, coverUrl, true));

        foreach (var mediaItem in service.MediaItems.OrderBy(m => m.DisplayOrder))
        {
            var url = await mediaService.ResolveUrlAsync(mediaItem.MediaAssetId, cancellationToken);
            if (url is not null)
                images.Add(new ServiceImageViewModel(mediaItem.Id, url, false));
        }

        return images;
    }

    private async Task<Dictionary<Guid, string>> GetCategoryTitlesAsync(CancellationToken cancellationToken)
    {
        var categories = await categoryService.GetAllAsync(0, SinglePageSize, cancellationToken);
        return categories.ToDictionary(c => c.Id, c => c.Title);
    }
}
