using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.SiteContent.Domain.Gallery;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.Abstractions;

public interface IGalleryCategoryRepository : IAsyncRepository<GalleryCategory, Guid>
{
}
