using OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataAccess.Abstractions.IRepositories;

public interface IBasketRepository : IGenericRepository<Basket>
{
    Task<Basket?> GetBasketByUserIdAsync(string id, bool tracking = true, bool? status = true);
}
