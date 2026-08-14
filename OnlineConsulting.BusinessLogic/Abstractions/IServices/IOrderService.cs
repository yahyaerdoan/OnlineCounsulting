using OnlineConsulting.BusinessLogic.Abstractions.IGenericServices;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.OrderDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;

namespace OnlineConsulting.BusinessLogic.Abstractions.IServices;

public interface IOrderService : IGenericService<Order, IDto>
{
    Task<IOperationResult> CreateOrderFromBasketAsync(CreateOrderDto dto);
    Task<IOperationResult<List<ResultOrderDto>>> GetOrdersByUserIdAsync(Guid userId);
    Task<IOperationResult<ResultOrderDetailDto>> GetOrderDetailByIdAsync(Guid orderId, Guid userId, bool tracking = true, bool? status = true);
    Task<IOperationResult<int>> GetTotalOrderCountAsync(Guid userId, bool tracking = true, bool? status = true);
    Task<IOperationResult<int>> GetOrderCountByOrderStatusAsync(Guid userId, string orderStatus, bool tracking = true, bool? status = true);
    Task<IOperationResult<int>> GetOrderCountByPaymentStatusAsync(Guid userId, string paymentStatus, bool tracking = true, bool? status = true);
    Task<IOperationResult<decimal>> GetTotalSpentByUserIdAsync(Guid userId, bool tracking = true, bool? status = true);
    Task<IOperationResult<IQueryable<TDto>>> GetAllOrderWithUsersAsync<TDto>(bool tracking = true, bool? status = true);
}
public interface IOrderNumberGenerator
{
    string Generate();
}
