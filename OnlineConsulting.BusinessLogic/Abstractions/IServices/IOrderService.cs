using OnlineConsulting.BusinessLogic.Abstractions.IGenericServices;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.OrderDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;

namespace OnlineConsulting.BusinessLogic.Abstractions.IServices;

public interface IOrderService : IGenericService<Order, IDto>
{
    Task<IOperationResult> CreateOrderFromBasketAsync(CreateOrderDto dto);
    Task<IOperationResult<List<ResultOrderDto>>> GetOrdersByUserIdAsync(string userId);
    Task<IOperationResult<ResultOrderDetailDto>> GetOrderDetailByIdAsync(Guid orderId, string userId, bool tracking = true, bool? status = true);
    Task<IOperationResult<int>> GetTotalOrderCountAsync(string userId, bool tracking = true, bool? status = true);
    Task<IOperationResult<int>> GetOrderCountByOrderStatusAsync(string userId, string orderStatus, bool tracking = true, bool? status = true);
    Task<IOperationResult<int>> GetOrderCountByPaymentStatusAsync(string userId, string paymentStatus, bool tracking = true, bool? status = true);
    Task<IOperationResult<decimal>> GetTotalSpentByUserIdAsync(string userId, bool tracking = true, bool? status = true);
    Task<IOperationResult<IQueryable<TDto>>> GetAllOrderWithUsersAsync<TDto>(bool tracking = true, bool? status = true);
}
public interface IOrderNumberGenerator
{
    string Generate();
}
