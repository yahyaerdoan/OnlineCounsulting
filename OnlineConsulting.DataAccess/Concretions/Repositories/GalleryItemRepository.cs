using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OnlineConsulting.DataAccess.Abstractions.IRepositories;
using OnlineConsulting.DataAccess.Concretions.Contexts;
using OnlineConsulting.DataAccess.Concretions.GenericRepositories;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataAccess.Concretions.Repositories;

public class GalleryItemRepository(OnlineConsultingDbContext context) : GenericRepository<GalleryItem>(context), IGalleryItemRepository
{
    public async Task<bool> AddGalleryItemAsync(GalleryItem entity)
    {
        EnsureSequentialId(entity);
        var entityEntry = await Entity.AddAsync(entity);
        return entityEntry.State == EntityState.Added;
    }

    public IQueryable<GalleryItem> GetAllGalleryItemWithCategories(bool traking = true, bool? status = true)
    {
        var query = Entity.AsQueryable();

        if (status.HasValue)
            query = query.Where(e => e.Status == status);

        query = query.Include(g => g.GalleryCategories)
                     .ThenInclude(gc => gc.GalleryCategory);

        if (!traking)
            query = query.AsNoTracking();

        return query;
    }

    public GalleryItem? GetByIdGalleryItemWithCategories(string id, bool tracking = true, bool? status = true)
    {
        IQueryable<GalleryItem> query = Entity;

        if (status.HasValue)
            query = query.Where(e => e.Status == status);

        query = query.Include(g => g.GalleryCategories)
                     .ThenInclude(gc => gc.GalleryCategory);

        if (!tracking)
            query = query.AsNoTracking();

        return query.FirstOrDefault(data => data.Id == Guid.Parse(id) && data.Status == status);
    }

    public async Task<bool> RemoveGalleryItemAsync(GalleryItem item, bool isSoftDelete = true)
    {
        if (isSoftDelete)
        {
            item.Status = false;
            var entityEntry = await Task.Run(() => Entity.Update(item));
            return true;
        }
        else
        {
            await RemoveGalleryItemsCategory(item);
            var entityEntry = await Task.Run(() => Entity.Remove(item));
            return entityEntry.State == EntityState.Deleted;
        }
    }
    public Task<bool> RemoveGalleryItemsCategory(GalleryItem item)
    {
        foreach (var category in item.GalleryCategories.ToList())
        {
            _context.Set<GalleryItemCategory>().Remove(category);
        }
        return Task.FromResult(true);
    }
}
