using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Requests.Lists;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;
using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.ListOrders;

/// <summary>Every user's orders for staff, one page at a time, with per-order totals.</summary>
public record ListOrdersQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null)
    : IRequest<OperationDataResult<Paginate<AdminOrderResponse>>>, ISecureAddRequest, IDynamicListRequest
{
    public static IReadOnlySet<string> QueryableFields { get; } = new HashSet<string>([nameof(Order.OrderNumber), nameof(Order.CreatedDate)]);

    public string[] Roles => [CommerceOperationClaims.Admin, CommerceOperationClaims.Read, GlobalOperationClaims.SuperAdmin];
}

public class ListOrdersHandler(IOrderRepository orderRepository)
    : IRequestHandler<ListOrdersQuery, OperationDataResult<Paginate<AdminOrderResponse>>>
{
    public async Task<OperationDataResult<Paginate<AdminOrderResponse>>> Handle(ListOrdersQuery request, CancellationToken cancellationToken)
    {
        var paged = await orderRepository.Query().ToDynamicPaginateAsync(request, defaultOrderBy: o => o.CreatedDate, tieBreaker: o => o.Id, cancellationToken: cancellationToken);

        if (paged.Items.Count == 0)
        {
            return Result.Success(new Paginate<AdminOrderResponse> { Items = [], Index = paged.Index, Size = paged.Size, Count = paged.Count, Pages = paged.Pages }, "No orders found.");
        }

        var totalsByOrderId = await orderRepository.GetTotalsAsync([.. paged.Items.Select(o => o.Id)], cancellationToken);

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
