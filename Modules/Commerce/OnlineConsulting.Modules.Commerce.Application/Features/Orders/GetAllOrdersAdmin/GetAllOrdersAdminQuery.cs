using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.GetAllOrdersAdmin;

/// <summary>Admin-only listing of every user's orders (GetOrdersQuery scopes to the calling user, this one doesn't).</summary>
public record GetAllOrdersAdminQuery : IRequest<OperationDataResult<List<AdminOrderResponse>>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [GlobalOperationClaims.SuperAdmin];
}

public class GetAllOrdersAdminHandler(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository)
    : IRequestHandler<GetAllOrdersAdminQuery, OperationDataResult<List<AdminOrderResponse>>>
{
    public async Task<OperationDataResult<List<AdminOrderResponse>>> Handle(GetAllOrdersAdminQuery request, CancellationToken cancellationToken)
    {
        var orders = await orderRepository.GetListAsync(o => true, size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        if (orders.Items.Count == 0)
            return Result.Success(new List<AdminOrderResponse>(), "No orders found.");

        var orderIds = orders.Items.Select(o => o.Id).ToList();
        var items = await orderItemRepository.GetListAsync(i => orderIds.Contains(i.OrderId), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        var totalsByOrderId = items.Items.GroupBy(i => i.OrderId).ToDictionary(g => g.Key, g => g.Sum(i => i.TotalPrice));

        List<AdminOrderResponse> responses = [.. orders.Items.Select(o => AdminOrderResponse.FromDomain(o, totalsByOrderId.GetValueOrDefault(o.Id)))];

        return Result.Success(responses, "Orders retrieved successfully.");
    }
}
