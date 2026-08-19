using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain.Gallery;
using OnlineConsulting.Modules.SiteContent.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Repositories.Gallery;

public class GalleryItemCategoryRepository(SiteContentDbContext context) : EfRepositoryBase<GalleryItemCategory, Guid, SiteContentDbContext>(context), IGalleryItemCategoryRepository
{
}
