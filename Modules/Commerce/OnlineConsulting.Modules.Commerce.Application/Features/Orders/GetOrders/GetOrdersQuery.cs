using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.GetOrders;

/// <summary>The user's orders, newest first - the storefront shows the latest order at the top.</summary>
public record GetOrdersQuery(Guid UserId) : IRequest<OperationDataResult<List<OrderResponse>>>;

public class GetOrdersHandler(IOrderRepository orderRepository)
    : IRequestHandler<GetOrdersQuery, OperationDataResult<List<OrderResponse>>>
{
    public async Task<OperationDataResult<List<OrderResponse>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await orderRepository.GetAllAsync(o => o.UserId == request.UserId, orderBy: q => q.OrderByDescending(o => o.CreatedDate).ThenByDescending(o => o.Id), cancellationToken: cancellationToken);
        if (orders.Count == 0)
        {
            return Result.Success(new List<OrderResponse>(), "No orders found for this user.");
        }

        var totalsByOrderId = await orderRepository.GetTotalsAsync([.. orders.Select(o => o.Id)], cancellationToken);

        List<OrderResponse> responses = [.. orders.Select(o => OrderResponse.FromDomain(o, totalsByOrderId.GetValueOrDefault(o.Id)))];

        return Result.Success(responses, "Orders retrieved successfully.");
    }
}
