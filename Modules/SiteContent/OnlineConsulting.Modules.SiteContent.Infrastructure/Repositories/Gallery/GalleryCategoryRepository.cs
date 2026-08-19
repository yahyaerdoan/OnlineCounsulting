using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain.Gallery;
using OnlineConsulting.Modules.SiteContent.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Repositories.Gallery;

public class GalleryCategoryRepository(SiteContentDbContext context) : EfRepositoryBase<GalleryCategory, Guid, SiteContentDbContext>(context), IGalleryCategoryRepository
{
}
