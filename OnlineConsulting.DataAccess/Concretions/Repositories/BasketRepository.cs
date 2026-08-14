using Microsoft.EntityFrameworkCore;
using OnlineConsulting.DataAccess.Abstractions.IRepositories;
using OnlineConsulting.DataAccess.Concretions.Contexts;
using OnlineConsulting.DataAccess.Concretions.GenericRepositories;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataAccess.Concretions.Repositories;

public class BasketRepository(OnlineConsultingDbContext context) : GenericRepository<Basket>(context), IBasketRepository
{
    public async Task<Basket?> GetBasketByUserIdAsync(Guid id, bool tracking = true, bool? status = true)
    {
        IQueryable<Basket> query = Entity;

        query = query.Where(b => b.UserId == id);

        if (status.HasValue)
            query = query.Where(b => b.Status == status.Value);

        if (!tracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync();
    }
}
