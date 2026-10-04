using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.GetOrderStats;

/// <summary>The caller's order count and how much they paid; unpaid, cancelled and refunded orders don't count as spent.</summary>
public record GetOrderStatsQuery(Guid UserId) : IRequest<OperationDataResult<OrderStatsResponse>>;

public class GetOrderStatsHandler(IOrderRepository orderRepository) : IRequestHandler<GetOrderStatsQuery, OperationDataResult<OrderStatsResponse>>
{
    public async Task<OperationDataResult<OrderStatsResponse>> Handle(GetOrderStatsQuery request, CancellationToken cancellationToken)
    {
        var (totalOrders, totalSpent) = await orderRepository.GetStatsAsync(request.UserId, cancellationToken);

        return Result.Success(new OrderStatsResponse(totalOrders, totalSpent), "Order stats retrieved successfully.");
    }
}
