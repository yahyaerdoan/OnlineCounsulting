using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;

public interface IOrderRepository : IAsyncRepository<Order, Guid>
{
}
