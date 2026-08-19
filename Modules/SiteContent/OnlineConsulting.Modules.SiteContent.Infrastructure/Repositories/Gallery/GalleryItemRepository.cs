using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain.Gallery;
using OnlineConsulting.Modules.SiteContent.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Repositories.Gallery;

public class GalleryItemRepository(SiteContentDbContext context) : EfRepositoryBase<GalleryItem, Guid, SiteContentDbContext>(context), IGalleryItemRepository
{
}
