using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Abstractions;

public interface IBasketItemRepository : IAsyncRepository<BasketItem, Guid>
{
}
