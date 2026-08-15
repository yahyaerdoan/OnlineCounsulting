using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.GetOrderDetail;

public record GetOrderDetailQuery(Guid OrderId, Guid UserId) : IRequest<OperationDataResult<OrderDetailResponse>>;

public class GetOrderDetailHandler(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository)
    : IRequestHandler<GetOrderDetailQuery, OperationDataResult<OrderDetailResponse>>
{
    public async Task<OperationDataResult<OrderDetailResponse>> Handle(GetOrderDetailQuery request, CancellationToken cancellationToken)
    {
        // Read-only lookup - no mutation follows, so track nothing.
        var order = await orderRepository.GetAsync(o => o.Id == request.OrderId && o.UserId == request.UserId, enableTracking: false, cancellationToken: cancellationToken);
        if (order is null)
            return Result.NotFound<OrderDetailResponse>($"Order {request.OrderId} was not found.");

        var items = await orderItemRepository.GetListAsync(i => i.OrderId == order.Id, size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        var totalPrice = items.Items.Sum(i => i.TotalPrice);

        var response = new OrderDetailResponse(OrderResponse.FromDomain(order, totalPrice), [.. items.Items.Select(OrderItemResponse.FromDomain)], order.ShippingAddressId, order.InvoiceAddressId);

        return Result.Success(response, "Order detail retrieved successfully.");
    }
}
