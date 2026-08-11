using Microsoft.EntityFrameworkCore;
using OnlineConsulting.DataAccess.Abstractions.IRepositories;
using OnlineConsulting.DataAccess.Concretions.Contexts;
using OnlineConsulting.DataAccess.Concretions.GenericRepositories;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataAccess.Concretions.Repositories;

public class ProvidedItemRepository(OnlineConsultingDbContext context) : GenericRepository<ProvidedItem>(context), IProvidedItemRepository
{
    public IQueryable<ProvidedItem> GetAllProvidedItemsWithImgIcons(bool traking = true, bool? status = true)
    {
        var query = Entity.Include(s => s.ImgIcon).AsQueryable();
        if (!traking)
            query = query.AsNoTracking();
        if (status.HasValue)
            query = query.Where(e => e.Status == status);

        return query;
    }
}
