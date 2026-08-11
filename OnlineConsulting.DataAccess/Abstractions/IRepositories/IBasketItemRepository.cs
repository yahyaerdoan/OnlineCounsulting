using OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataAccess.Abstractions.IRepositories;

public interface IBasketItemRepository : IGenericRepository<BasketItem>
{
    Task<int> GetTotalBasketItemsCountAsync(Guid basketId, bool tracking = true, bool? status = true);
}
