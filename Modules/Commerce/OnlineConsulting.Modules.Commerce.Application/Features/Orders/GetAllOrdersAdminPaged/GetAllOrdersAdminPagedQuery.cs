using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.GetAllOrdersAdminPaged;

/// <summary>Paginated sibling of GetAllOrdersAdminQuery - same Super Admin gate, page-scoped order-item join.</summary>
public record GetAllOrdersAdminPagedQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null)
    : IRequest<OperationDataResult<Paginate<AdminOrderResponse>>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [GlobalOperationClaims.SuperAdmin];
}

public class GetAllOrdersAdminPagedHandler(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository)
    : IRequestHandler<GetAllOrdersAdminPagedQuery, OperationDataResult<Paginate<AdminOrderResponse>>>
{
    public async Task<OperationDataResult<Paginate<AdminOrderResponse>>> Handle(GetAllOrdersAdminPagedQuery request, CancellationToken cancellationToken)
    {
        var paged = await orderRepository.Query().ToDynamicPaginateAsync(request.PageRequest, request.DynamicQuery, defaultOrderBy: o => o.CreatedDate, tieBreaker: o => o.Id, cancellationToken);

        if (paged.Items.Count == 0)
        {
            return Result.Success(new Paginate<AdminOrderResponse> { Items = [], Index = paged.Index, Size = paged.Size, Count = paged.Count, Pages = paged.Pages }, "No orders found.");
        }

        var orderIds = paged.Items.Select(o => o.Id).ToList();
        var items = await orderItemRepository.GetListAsync(i => orderIds.Contains(i.OrderId), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        var totalsByOrderId = items.Items.GroupBy(i => i.OrderId).ToDictionary(g => g.Key, g => g.Sum(i => i.TotalPrice));

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
