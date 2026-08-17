using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;
using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.Modules.Commerce.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Commerce.Infrastructure.Repositories;

public class OrderItemRepository(CommerceDbContext context) : EfRepositoryBase<OrderItem, Guid, CommerceDbContext>(context), IOrderItemRepository
{
}
