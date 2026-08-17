using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;

public interface IOrderItemRepository : IAsyncRepository<OrderItem, Guid>
{
}
