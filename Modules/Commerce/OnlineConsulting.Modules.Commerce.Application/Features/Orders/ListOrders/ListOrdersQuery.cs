using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.ListOrders;

/// <summary>Every user's orders for staff, one page at a time, with per-order totals.</summary>
public record ListOrdersQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null)
    : IRequest<OperationDataResult<Paginate<AdminOrderResponse>>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [CommerceOperationClaims.Admin, CommerceOperationClaims.Read, GlobalOperationClaims.SuperAdmin];
}

public class ListOrdersHandler(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository)
    : IRequestHandler<ListOrdersQuery, OperationDataResult<Paginate<AdminOrderResponse>>>
{
    public async Task<OperationDataResult<Paginate<AdminOrderResponse>>> Handle(ListOrdersQuery request, CancellationToken cancellationToken)
    {
        var paged = await orderRepository.Query().ToDynamicPaginateAsync(request.PageRequest, request.DynamicQuery, defaultOrderBy: o => o.CreatedDate, tieBreaker: o => o.Id, cancellationToken);

        if (paged.Items.Count == 0)
        {
            return Result.Success(new Paginate<AdminOrderResponse> { Items = [], Index = paged.Index, Size = paged.Size, Count = paged.Count, Pages = paged.Pages }, "No orders found.");
        }

        var orderIds = paged.Items.Select(o => o.Id).ToList();
        var items = await orderItemRepository.GetAllAsync(i => orderIds.Contains(i.OrderId), cancellationToken: cancellationToken);
        var totalsByOrderId = items.GroupBy(i => i.OrderId).ToDictionary(g => g.Key, g => g.Sum(i => i.TotalPrice));

        var response = new Paginate<AdminOrderResponse>
        {
            Items = [.. paged.Items.Select(o => AdminOrderResponse.FromDomain(o, totalsByOrderId.GetValueOrDefault(o.Id)))],
            Index = paged.Index,
            Size = paged.Size,
            Count = paged.Count,
            Pages = paged.Pages,
        };

        return Result.Success(response, "Orders retrieved successfully.");
    }
}
