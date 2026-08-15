using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.SiteContent.Application;
using OnlineConsulting.Modules.SiteContent.Domain;
using OnlineConsulting.Modules.SiteContent.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Repositories;

public class GalleryCategoryRepository(SiteContentDbContext context) : EfRepositoryBase<GalleryCategory, Guid, SiteContentDbContext>(context), IGalleryCategoryRepository
{
}
