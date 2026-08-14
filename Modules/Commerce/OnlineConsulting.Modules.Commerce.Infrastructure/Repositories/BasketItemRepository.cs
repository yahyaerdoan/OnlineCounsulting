using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Contracts;
using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.Modules.Commerce.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Commerce.Infrastructure.Repositories;

public class BasketItemRepository(CommerceDbContext context) : EfRepositoryBase<BasketItem, Guid, CommerceDbContext>(context), IBasketItemRepository
{
}
