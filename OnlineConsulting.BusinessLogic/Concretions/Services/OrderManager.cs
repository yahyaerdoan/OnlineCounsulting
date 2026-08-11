using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ResultHandler.Implementations.Error;
using ResultHandler.Implementations.Success;
using OnlineConsulting.BusinessLogic.Abstractions.IServices;
using OnlineConsulting.BusinessLogic.Concretions.GenericServices;
using OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;
using OnlineConsulting.DataAccess.Abstractions.IRepositories;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.OrderDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.OrderItemDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.ServiceDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.UserAddressDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;
using ResultHandler.Core.Enums;

namespace OnlineConsulting.BusinessLogic.Concretions.Services;

public class OrderManager(IMapper mapper, IGenericRepository<Order> repository, IOrderRepository orderRepository, IOrderNumberGenerator orderNumberGenerator, IBasketItemService basketItemService, IOrderItemService orderItemService, IBasketService basketService, IUserAddressService userAddressService) : GenericService<Order, IDto>(mapper, repository), IOrderService
{
    public async Task<IOperationResult> CreateOrderFromBasketAsync(CreateOrderDto dto)
    {
        var basketResult = await basketItemService.GetBasketItemsByUserIdAsync(dto.UserId, false, true);
        if (!basketResult.IsSuccessful || basketResult.Data is null || !await basketResult.Data.AnyAsync())
            return new ErrorResult("Basket not found or is empty.", ResultStatus.BadRequest);

        var resultShipping = await userAddressService.GetShippingAddressAsync();
        var resultBilling = await userAddressService.GetBillingAddressAsync();

        if (!resultShipping.IsSuccessful || resultShipping.Data is null)
            return new ErrorResult("Shipping address not found.", ResultStatus.BadRequest);

        if (!resultBilling.IsSuccessful || resultBilling.Data is null)
            return new ErrorResult("Billing address not found.", ResultStatus.BadRequest);

        dto.ShippingAddressId = resultShipping.Data.Id;
        dto.InvoiceAddressId = resultBilling.Data.Id;
        dto.OrderNumber = orderNumberGenerator.Generate();
        dto.OrderStatus = OrderStatus.Pending;
        dto.PaymentStatus = PaymentStatus.Paid;

        var orderIdResult = await CreateAndReturnIdAsync(dto);

        var basketItems = await basketResult.Data.ToListAsync();

        var orderItems = _mapper.Map<List<CreateOrderItemDto>>(basketItems);
        foreach (var item in orderItems)
            item.OrderId = orderIdResult.Data;

        await orderItemService.AddRangeAsync([.. orderItems.Cast<IDto>()]);

        var basketId = basketItems.First().BasketId;

        if (basketId != Guid.Empty)
        {
            await basketItemService.ClearBasketItemsByIdAsync(basketId);
            await basketService.DeleteBasketByIdAsync(basketId);
        }
        else
        {
            return new ErrorResult("Basket ID is invalid.", ResultStatus.BadRequest);
        }

        return new SuccessResult($"Order created: {dto.OrderNumber}", ResultStatus.Created);
    }

    public async Task<IOperationResult<IQueryable<TDto>>> GetAllOrderWithUsersAsync<TDto>(bool tracking = true, bool? status = true)
    {
        var result = orderRepository.GetAllOrderWithUsers(tracking, status)
              .Select(order => _mapper.Map<TDto>(order));
        if (result is null || !await result.AnyAsync())
            return new ErrorDataResult<IQueryable<TDto>>("No orders found.", ResultStatus.NotFound);
        return new SuccessDataResult<IQueryable<TDto>>(result, "Orders retrieved successfully.", ResultStatus.Ok);
    }

    public async Task<IOperationResult<int>> GetOrderCountByOrderStatusAsync(string userId, string orderStatus, bool tracking = true, bool? status = true)
    {
        var orderStatusCount = await orderRepository.GetOrderCountByOrderStatusAsync(userId, orderStatus, tracking, status);
        if (orderStatusCount == 0)
            return new ErrorDataResult<int>("No orders found with the specified status.", ResultStatus.NotFound);
        return new SuccessDataResult<int>(orderStatusCount, "Order count by status retrieved successfully.", ResultStatus.Ok);
    }

    public async Task<IOperationResult<int>> GetOrderCountByPaymentStatusAsync(string userId, string paymentStatus, bool tracking = true, bool? status = true)
    {
        var paymentStatusCount = await orderRepository.GetOrderCountByPaymentStatusAsync(userId, paymentStatus, tracking, status);
        if (paymentStatusCount == 0)
            return new ErrorDataResult<int>("No orders found with the specified status.", ResultStatus.NotFound);
        return new SuccessDataResult<int>(paymentStatusCount, "Order count by payment status retrieved successfully.", ResultStatus.Ok);
    }

    public async Task<IOperationResult<ResultOrderDetailDto>> GetOrderDetailByIdAsync(Guid orderId, string userId, bool tracking = true, bool? status = true)
    {
        var order = await orderRepository.GetOrderAndOrderItemDetailByIdAsync(orderId, userId, tracking, status);
        if (order is null)
            return new ErrorDataResult<ResultOrderDetailDto>("Order Detail not found.", ResultStatus.NotFound);

        // Map order and items to DTOs
        var orderDto = _mapper.Map<ResultOrderDto>(order);
        var orderItemDtos = _mapper.Map<List<ResultOrderItemDto>>(order.OrderItems);
        var shippingAddress = _mapper.Map<ResultUserAddressDto>(order.ShippingAddress);
        var invoiceAddress = _mapper.Map<ResultUserAddressDto>(order.InvoiceAddress);
        var serviceDtos = order.OrderItems.Select(oi => _mapper.Map<ResultServiceWithImageDto>(oi.Service)).ToList();

        var model = new ResultOrderDetailDto
        {
            Order = orderDto,
            OrderItems = orderItemDtos,
            Services = serviceDtos,
            ShippingAddress = shippingAddress,
            InvoiceAddress = invoiceAddress,
        };

        return new SuccessDataResult<ResultOrderDetailDto>(model, "Order Detail retrieved successfully", ResultStatus.Ok);
    }

    public async Task<IOperationResult<List<ResultOrderDto>>> GetOrdersByUserIdAsync(string userId)
    {
        var queryResult = await GetWhereAsync<ResultOrderDto>(order => order.UserId == userId);

        if (!queryResult.IsSuccessful || queryResult.Data is null)
            return new ErrorDataResult<List<ResultOrderDto>>("No orders found for this user.", ResultStatus.NotFound);

        var orderList = await queryResult.Data.ToListAsync();

        return new SuccessDataResult<List<ResultOrderDto>>(orderList, "Orders retrieved successfully.", ResultStatus.Ok);
    }

    public async Task<IOperationResult<int>> GetTotalOrderCountAsync(string userId, bool tracking = true, bool? status = true)
    {
        var totalCount = await orderRepository.GetTotalOrderCountAsync(userId, tracking, status);
        if (totalCount == 0)
            return new ErrorDataResult<int>("No orders found.", ResultStatus.NotFound);
        return new SuccessDataResult<int>(totalCount, "Total order count retrieved successfully.", ResultStatus.Ok);
    }

    public async Task<IOperationResult<decimal>> GetTotalSpentByUserIdAsync(string userId, bool tracking = true, bool? status = true)
    {
        var totalSpent = await orderRepository.GetTotalSpentByUserIdAsync(userId, tracking, status);
        if (totalSpent == 0)
            return new ErrorDataResult<decimal>("No orders found for this user.", ResultStatus.NotFound);
        return new SuccessDataResult<decimal>(totalSpent, "Total spent amount retrieved successfully.", ResultStatus.Ok);
    }
}

public class GuidOrderNumberGenerator : IOrderNumberGenerator
{
    public string Generate() => $"ORD-{Guid.NewGuid().ToString()[..8].ToUpper()}";
}
public static class OrderStatus
{
    public const string Pending = "Pending";
    public const string Cancelled = "Cancelled";
}
public static class PaymentStatus
{
    public const string Paid = "Paid";
    public const string Cancelled = "Cancelled";
    public const string Refunded = "Refunded";
}
