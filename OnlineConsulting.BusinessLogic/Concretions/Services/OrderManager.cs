using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
using OnlineConsulting.Modules.Identity.Domain;
using ResultHandler.Core.Abstractions;
using ResultHandler.Core.Enums;
using ResultHandler.Implementations.Error;
using ResultHandler.Implementations.Success;

namespace OnlineConsulting.BusinessLogic.Concretions.Services;

public class OrderManager(IMapper mapper, IGenericRepository<Order> repository, IOrderRepository orderRepository, IOrderNumberGenerator orderNumberGenerator, IBasketItemService basketItemService, IOrderItemService orderItemService, IBasketService basketService, IUserAddressService userAddressService, UserManager<User> userManager) : GenericService<Order, IDto>(mapper, repository), IOrderService
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
        var orders = await orderRepository.GetAllOrderWithUsers(tracking, status).ToListAsync();
        if (orders.Count == 0)
            return new ErrorDataResult<IQueryable<TDto>>("No orders found.", ResultStatus.NotFound);

        var mapped = orders.Select(order => _mapper.Map<TDto>(order)).ToList();

        // Order (this DbContext) and User (Identity module's own DbContext) can't be EF-joined
        // anymore, so backfill display names with a second lookup when the caller asked for
        // ResultOrderDto specifically.
        if (mapped is List<ResultOrderDto> resultOrders)
        {
            var userIds = orders.Select(o => o.UserId).Distinct().ToList();
            var usersById = await userManager.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id);

            foreach (var (order, dto) in orders.Zip(resultOrders))
            {
                if (usersById.TryGetValue(order.UserId, out var user))
                {
                    dto.UserFirstName = user.FirstName;
                    dto.UserLastName = user.LastName;
                }
            }
        }

        return new SuccessDataResult<IQueryable<TDto>>(mapped.AsQueryable(), "Orders retrieved successfully.", ResultStatus.Ok);
    }

    public async Task<IOperationResult<int>> GetOrderCountByOrderStatusAsync(Guid userId, string orderStatus, bool tracking = true, bool? status = true)
    {
        var orderStatusCount = await orderRepository.GetOrderCountByOrderStatusAsync(userId, orderStatus, tracking, status);
        if (orderStatusCount == 0)
            return new ErrorDataResult<int>("No orders found with the specified status.", ResultStatus.NotFound);
        return new SuccessDataResult<int>(orderStatusCount, "Order count by status retrieved successfully.", ResultStatus.Ok);
    }

    public async Task<IOperationResult<int>> GetOrderCountByPaymentStatusAsync(Guid userId, string paymentStatus, bool tracking = true, bool? status = true)
    {
        var paymentStatusCount = await orderRepository.GetOrderCountByPaymentStatusAsync(userId, paymentStatus, tracking, status);
        if (paymentStatusCount == 0)
            return new ErrorDataResult<int>("No orders found with the specified status.", ResultStatus.NotFound);
        return new SuccessDataResult<int>(paymentStatusCount, "Order count by payment status retrieved successfully.", ResultStatus.Ok);
    }

    public async Task<IOperationResult<ResultOrderDetailDto>> GetOrderDetailByIdAsync(Guid orderId, Guid userId, bool tracking = true, bool? status = true)
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

    public async Task<IOperationResult<List<ResultOrderDto>>> GetOrdersByUserIdAsync(Guid userId)
    {
        var queryResult = await GetWhereAsync<ResultOrderDto>(order => order.UserId == userId);

        if (!queryResult.IsSuccessful || queryResult.Data is null)
            return new ErrorDataResult<List<ResultOrderDto>>("No orders found for this user.", ResultStatus.NotFound);

        var orderList = await queryResult.Data.ToListAsync();

        return new SuccessDataResult<List<ResultOrderDto>>(orderList, "Orders retrieved successfully.", ResultStatus.Ok);
    }

    public async Task<IOperationResult<int>> GetTotalOrderCountAsync(Guid userId, bool tracking = true, bool? status = true)
    {
        var totalCount = await orderRepository.GetTotalOrderCountAsync(userId, tracking, status);
        if (totalCount == 0)
            return new ErrorDataResult<int>("No orders found.", ResultStatus.NotFound);
        return new SuccessDataResult<int>(totalCount, "Total order count retrieved successfully.", ResultStatus.Ok);
    }

    public async Task<IOperationResult<decimal>> GetTotalSpentByUserIdAsync(Guid userId, bool tracking = true, bool? status = true)
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
