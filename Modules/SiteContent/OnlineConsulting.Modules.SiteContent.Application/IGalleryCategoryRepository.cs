using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.SiteContent.Domain;

namespace OnlineConsulting.Modules.SiteContent.Application;

public interface IGalleryCategoryRepository : IAsyncRepository<GalleryCategory, Guid>
{
}
