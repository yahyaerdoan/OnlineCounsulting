using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.GetOrderStats;

public record GetOrderStatsQuery(Guid UserId) : IRequest<OperationDataResult<OrderStatsResponse>>;

public class GetOrderStatsHandler(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository)
    : IRequestHandler<GetOrderStatsQuery, OperationDataResult<OrderStatsResponse>>
{
    public async Task<OperationDataResult<OrderStatsResponse>> Handle(GetOrderStatsQuery request, CancellationToken cancellationToken)
    {
        var orders = await orderRepository.GetListAsync(o => o.UserId == request.UserId, size: int.MaxValue, cancellationToken: cancellationToken);
        if (orders.Items.Count == 0)
            return Result.Success(new OrderStatsResponse(0, 0), "No orders found for this user.");

        var orderIds = orders.Items.Select(o => o.Id).ToList();
        var items = await orderItemRepository.GetListAsync(i => orderIds.Contains(i.OrderId), size: int.MaxValue, cancellationToken: cancellationToken);
        var totalSpent = items.Items.Sum(i => i.TotalPrice);

        return Result.Success(new OrderStatsResponse(orders.Items.Count, totalSpent), "Order stats retrieved successfully.");
    }
}
