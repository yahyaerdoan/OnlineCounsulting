using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.SiteContent.Domain.Gallery;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.Abstractions;

public interface IGalleryItemCategoryRepository : IAsyncRepository<GalleryItemCategory, Guid>
{
}
