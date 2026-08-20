using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.GetOrders;

public record GetOrdersQuery(Guid UserId) : IRequest<OperationDataResult<List<OrderResponse>>>;

public class GetOrdersHandler(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository)
    : IRequestHandler<GetOrdersQuery, OperationDataResult<List<OrderResponse>>>
{
    public async Task<OperationDataResult<List<OrderResponse>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await orderRepository.GetListAsync(o => o.UserId == request.UserId, size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        if (orders.Items.Count == 0)
        {
            return Result.Success(new List<OrderResponse>(), "No orders found for this user.");
        }

        var orderIds = orders.Items.Select(o => o.Id).ToList();
        var items = await orderItemRepository.GetListAsync(i => orderIds.Contains(i.OrderId), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        var totalsByOrderId = items.Items.GroupBy(i => i.OrderId).ToDictionary(g => g.Key, g => g.Sum(i => i.TotalPrice));

        List<OrderResponse> responses = [.. orders.Items.Select(o => OrderResponse.FromDomain(o, totalsByOrderId.GetValueOrDefault(o.Id)))];

        return Result.Success(responses, "Orders retrieved successfully.");
    }
}
