using Microsoft.EntityFrameworkCore;
using OnlineConsulting.DataAccess.Abstractions.IRepositories;
using OnlineConsulting.DataAccess.Concretions.Contexts;
using OnlineConsulting.DataAccess.Concretions.GenericRepositories;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataAccess.Concretions.Repositories;

public class ServiceRepository(OnlineConsultingDbContext context) : GenericRepository<Service>(context), IServiceRepository
{
    public IQueryable<Service> GetAllServicesWithImages(bool traking = true, bool? status = true)
    {
        var query = Entity.Include(s => s.ServiceImages).Include(s => s.Category).AsQueryable();
        if (!traking)
            query = query.AsNoTracking();
        if (status.HasValue)
            query = query.Where(e => e.Status == status);

        return query;
    }
    public IQueryable<Service> GetAllServicesByFeaturedAreaTrue(bool traking = true, bool? status = true)
    {
        var query = Entity.Include(s => s.ServiceImages).Include(s => s.Category).AsQueryable();
        if (!traking)
            query = query.AsNoTracking();
        if (status.HasValue)
            query = query.Where(e => e.Status == status && e.FeaturedArea);

        return query;
    }
    public IQueryable<Service> GetAllServicesWithPaged(int size, int page, bool traking = true, bool? status = true)
    {
        var query = Entity.Include(s => s.ServiceImages).Include(s => s.Category).Skip((page - 1) * size).Take(size).AsQueryable();
        if (!traking)
            query = query.AsNoTracking();
        if (status.HasValue)
            query = query.Where(e => e.Status == status);

        return query;
    }
    public IQueryable<Service> GetServicesWithImagesByCategoryIdAsync(string id, bool tracking = true, bool? status = true)
    {
        var query = Entity
            .Include(s => s.ServiceImages)
            .Include(s => s.Category).AsQueryable()
            .Where(s => s.CategoryId == Guid.Parse(id));
        if (!tracking)
            query = query.AsNoTracking();

        if (!status.HasValue)
            query = query.Where(s => s.Status == status);

        return query;
    }
    public async Task<Service?> GetServiceWithImagesByIdAsync(string id, bool traking = true, bool? status = true)
    {
        var query = Entity.Include(s => s.ServiceImages).Include(s => s.Category).AsQueryable();
        if (!traking)
            query = query.AsNoTracking();
        return await query.FirstOrDefaultAsync(data => data.Id == Guid.Parse(id) && data.Status == status);
    }
    public async Task<Service?> GetServiceBySlugWithImagesAndCategorydAsync(string slug, bool traking = true, bool? status = true)
    {
        var query = Entity.Include(s => s.ServiceImages).Include(s => s.Category).AsQueryable();
        if (!traking)
            query = query.AsNoTracking();
        return await query.FirstOrDefaultAsync(data => data.Slug == slug && data.Status == status);
    }
    public IQueryable<Service> SearchServicesWithImages(string query, bool traking = true, bool? status = true)
    {
        var services = Entity.Include(s => s.ServiceImages).Include(s => s.Category).AsQueryable();
        if (!traking)
            services = services.AsNoTracking();
        if (status.HasValue)
            services = services.Where(e => e.Status == status);

        return services.Where(s => EF.Functions.Like(s.Title, $"%{query}%") || EF.Functions.Like(s.Description, $"%{query}%"));
    }
}
