namespace OnlineConsulting.UserInterface.Areas.User.Features.Order;

/// <summary>Self-service order history/detail/stats for the current logged-in user via /api/orders*.</summary>
public interface IOrderService
{
    /// <summary>Gets all orders for the current user.</summary>
    Task<List<OrderResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets order detail by id, scoped to the current user.</summary>
    /// <returns>Null if not found or not owned by the caller.</returns>
    Task<OrderDetailResponse?> GetDetailAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Gets order totals/spend stats for the current user.</summary>
    Task<OrderStatsResponse?> GetStatsAsync(CancellationToken cancellationToken = default);
}

public record OrderResponse(Guid Id, string OrderNumber, string OrderStatus, string PaymentStatus, decimal TotalPrice, DateTimeOffset CreatedDate);

public record OrderItemResponse(Guid Id, Guid ServiceId, int Quantity, decimal UnitPrice, int TaxRate, decimal TaxAmount, decimal SubTotalPrice, decimal TotalPrice);

public record OrderDetailResponse(OrderResponse Order, List<OrderItemResponse> Items, Guid ShippingAddressId, Guid InvoiceAddressId);

public record OrderStatsResponse(int TotalOrders, decimal TotalSpent);
