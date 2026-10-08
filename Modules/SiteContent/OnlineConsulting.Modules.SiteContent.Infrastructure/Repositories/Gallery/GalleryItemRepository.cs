using Core.PersistenceLayer.Repositories.EfRepositories;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain.Gallery;
using OnlineConsulting.Modules.SiteContent.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Repositories.Gallery;

public class GalleryItemRepository(SiteContentDbContext context) : EfRepositoryBase<GalleryItem, Guid, SiteContentDbContext>(context), IGalleryItemRepository
{
    public Task<GalleryItem?> GetWithCategoriesAsync(Guid id, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        var query = QueryWithCategories();
        if (!enableTracking)
        {
            query = query.AsNoTracking();
        }

        return query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public IQueryable<GalleryItem> QueryWithCategories() => Context.GalleryItems.Include(x => x.Categories);
}
