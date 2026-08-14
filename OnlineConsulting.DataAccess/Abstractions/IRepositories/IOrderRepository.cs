using OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataAccess.Abstractions.IRepositories;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<Order?> GetOrderAndOrderItemDetailByIdAsync(Guid orderId, Guid userId, bool tracking = true, bool? status = true);
    Task<int> GetTotalOrderCountAsync(Guid userId, bool tracking = true, bool? status = true);
    Task<decimal> GetTotalSpentByUserIdAsync(Guid userId, bool tracking = true, bool? status = true);
    Task<int> GetOrderCountByOrderStatusAsync(Guid userId, string orderStatus, bool tracking = true, bool? status = true);
    Task<int> GetOrderCountByPaymentStatusAsync(Guid userId, string paymentStatus, bool tracking = true, bool? status = true);
    IQueryable<Order> GetAllOrderWithUsers(bool traking = true, bool? status = true);
}
