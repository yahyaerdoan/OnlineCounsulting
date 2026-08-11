using Microsoft.EntityFrameworkCore;
using OnlineConsulting.DataAccess.Abstractions.IRepositories;
using OnlineConsulting.DataAccess.Concretions.Contexts;
using OnlineConsulting.DataAccess.Concretions.GenericRepositories;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataAccess.Concretions.Repositories;

public class BasketItemRepository(OnlineConsultingDbContext context) : GenericRepository<BasketItem>(context), IBasketItemRepository
{
    public Task<int> GetTotalBasketItemsCountAsync(Guid basketId, bool tracking = true, bool? status = true)
    {
        var query = Entity.Where(o => o.BasketId == basketId);
        if (status.HasValue)
            query = query.Where(o => o.Status == status.Value);

        if (!tracking)
            query = query.AsNoTracking();

        return query.CountAsync();
    }
}
