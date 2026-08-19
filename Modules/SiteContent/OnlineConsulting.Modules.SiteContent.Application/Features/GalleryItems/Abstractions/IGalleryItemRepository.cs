using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.SiteContent.Domain.Gallery;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.Abstractions;

public interface IGalleryItemRepository : IAsyncRepository<GalleryItem, Guid>
{
}
