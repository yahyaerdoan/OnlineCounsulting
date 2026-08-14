using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Contracts;

public interface IBasketRepository : IAsyncRepository<Basket, Guid>
{
}
