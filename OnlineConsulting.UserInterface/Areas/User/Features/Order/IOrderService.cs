namespace OnlineConsulting.UserInterface.Areas.User.Features.Order;

/// <summary>Self-service order history/detail/stats for the current logged-in user via /api/orders*.</summary>
public interface IOrderService
{
    Task<List<OrderResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<OrderDetailResponse?> GetDetailAsync(Guid id, CancellationToken cancellationToken = default);
    Task<OrderStatsResponse?> GetStatsAsync(CancellationToken cancellationToken = default);
}

public record OrderResponse(Guid Id, string OrderNumber, string OrderStatus, string PaymentStatus, decimal TotalPrice, DateTimeOffset CreatedDate);

public record OrderItemResponse(Guid Id, Guid ServiceId, int Quantity, decimal UnitPrice, int TaxRate, decimal TaxAmount, decimal SubTotalPrice, decimal TotalPrice);

public record OrderDetailResponse(OrderResponse Order, List<OrderItemResponse> Items, Guid ShippingAddressId, Guid InvoiceAddressId);

public record OrderStatsResponse(int TotalOrders, decimal TotalSpent);
