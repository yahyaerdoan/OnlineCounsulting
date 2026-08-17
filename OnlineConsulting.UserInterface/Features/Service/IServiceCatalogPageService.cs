namespace OnlineConsulting.UserInterface.Features.Service;

/// <summary>Api orchestration for the public service catalog/detail pages - composes IServiceCatalogService,
/// ICategoryService (title resolution) and IMediaService (cover/gallery url resolution) so the ViewComponents
/// only ever call this one interface.</summary>
public interface IServiceCatalogPageService
{
    Task<ServiceListViewModel> GetPagedAsync(int page, int size, CancellationToken cancellationToken = default);
    Task<ServiceDetailViewModel?> GetDetailAsync(string slug, CancellationToken cancellationToken = default);
    Task<List<ServiceCardViewModel>> GetRelatedAsync(string slug, CancellationToken cancellationToken = default);
}
