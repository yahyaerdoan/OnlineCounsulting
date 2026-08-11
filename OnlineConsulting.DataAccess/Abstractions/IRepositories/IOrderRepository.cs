using OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataAccess.Abstractions.IRepositories;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<Order?> GetOrderAndOrderItemDetailByIdAsync(Guid orderId, string userId, bool tracking = true, bool? status = true);
    Task<int> GetTotalOrderCountAsync(string userId, bool tracking = true, bool? status = true);
    Task<decimal> GetTotalSpentByUserIdAsync(string userId, bool tracking = true, bool? status = true);
    Task<int> GetOrderCountByOrderStatusAsync(string userId, string orderStatus, bool tracking = true, bool? status = true);
    Task<int> GetOrderCountByPaymentStatusAsync(string userId, string paymentStatus, bool tracking = true, bool? status = true);
    IQueryable<Order> GetAllOrderWithUsers(bool traking = true, bool? status = true);
}
