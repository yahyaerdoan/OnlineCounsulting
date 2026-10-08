using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.GetOrderDetail;

public record GetOrderDetailQuery(Guid OrderId, Guid UserId) : IRequest<OperationDataResult<OrderDetailResponse>>;

public class GetOrderDetailHandler(IOrderRepository orderRepository) : IRequestHandler<GetOrderDetailQuery, OperationDataResult<OrderDetailResponse>>
{
    public async Task<OperationDataResult<OrderDetailResponse>> Handle(GetOrderDetailQuery request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetWithItemsAsync(o => o.Id == request.OrderId && o.UserId == request.UserId, enableTracking: false, cancellationToken: cancellationToken);

        if (order is null)
        {
            return Result.NotFound<OrderDetailResponse>($"Order {request.OrderId} was not found.");
        }

        var response = new OrderDetailResponse(OrderResponse.FromDomain(order, order.Items.Sum(i => i.TotalPrice)), [.. order.Items.Select(OrderItemResponse.FromDomain)],
            order.ShippingAddressId, order.InvoiceAddressId);

        return Result.Success(response, "Order detail retrieved successfully.");
    }
}
