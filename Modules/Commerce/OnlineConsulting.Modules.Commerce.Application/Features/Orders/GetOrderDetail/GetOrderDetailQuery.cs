using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.GetOrderDetail;

public record GetOrderDetailQuery(Guid OrderId, Guid UserId) : IRequest<OperationDataResult<OrderDetailResponse>>;

public class GetOrderDetailHandler(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository) : IRequestHandler<GetOrderDetailQuery, OperationDataResult<OrderDetailResponse>>
{
    public async Task<OperationDataResult<OrderDetailResponse>> Handle(GetOrderDetailQuery request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetAsync(o => o.Id == request.OrderId && o.UserId == request.UserId, enableTracking: false, cancellationToken: cancellationToken);

        if (order is null)
        {
            return Result.NotFound<OrderDetailResponse>($"Order {request.OrderId} was not found.");
        }

        var items = await orderItemRepository.GetAllAsync(i => i.OrderId == order.Id, cancellationToken: cancellationToken);
        var totalPrice = items.Sum(i => i.TotalPrice);

        var response = new OrderDetailResponse(OrderResponse.FromDomain(order, totalPrice), [.. items.Select(OrderItemResponse.FromDomain)], order.ShippingAddressId, order.InvoiceAddressId);

        return Result.Success(response, "Order detail retrieved successfully.");
    }
}
