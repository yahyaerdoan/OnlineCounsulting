using OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataAccess.Abstractions.IRepositories;

public interface IGalleryItemRepository : IGenericRepository<GalleryItem>
{
    IQueryable<GalleryItem> GetAllGalleryItemWithCategories(bool traking = true, bool? status = true);
    GalleryItem? GetByIdGalleryItemWithCategories(string id, bool traking = true, bool? status = true);
    Task<bool> AddGalleryItemAsync(GalleryItem entity);
    Task<bool> RemoveGalleryItemsCategory(GalleryItem item);
    Task<bool> RemoveGalleryItemAsync(GalleryItem item, bool isSoftDelete = true);
}
