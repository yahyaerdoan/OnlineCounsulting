namespace OnlineConsulting.UserInterface.Features.Service;

/// <summary>Composes IServiceCatalogService, ICategoryService and IMediaService so ViewComponents call only this one interface.</summary>
public interface IServiceCatalogPageService
{
    Task<ServiceListViewModel> GetPagedAsync(int page, int size, CancellationToken cancellationToken = default);
    Task<ServiceDetailViewModel?> GetDetailAsync(string slug, CancellationToken cancellationToken = default);
    Task<List<ServiceCardViewModel>> GetRelatedAsync(string slug, CancellationToken cancellationToken = default);
}
