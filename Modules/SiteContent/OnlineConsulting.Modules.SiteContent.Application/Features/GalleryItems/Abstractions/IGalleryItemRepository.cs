using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.SiteContent.Domain.Gallery;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.Abstractions;

public interface IGalleryItemRepository : IAsyncRepository<GalleryItem, Guid>
{
    /// <summary>The item loaded with its category links, for showing or changing it.</summary>
    Task<GalleryItem?> GetWithCategoriesAsync(Guid id, bool enableTracking = true, CancellationToken cancellationToken = default);

    /// <summary>Items with their category links, for list queries to filter, sort and page.</summary>
    IQueryable<GalleryItem> QueryWithCategories();
}
