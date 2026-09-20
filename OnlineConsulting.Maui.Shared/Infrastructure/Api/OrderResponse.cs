namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors GET /api/orders's response shape (the caller's own orders).</summary>
public record OrderResponse(Guid Id, string OrderNumber, string OrderStatus, string PaymentStatus, decimal TotalPrice, DateTimeOffset CreatedDate);

/// <summary>Mirrors GET /api/orders/{id}'s response shape.</summary>
public record OrderDetailResponse(OrderResponse Order, IReadOnlyList<OrderItemResponse> Items, Guid ShippingAddressId, Guid InvoiceAddressId);

public record OrderItemResponse(Guid Id, Guid ServiceId, int Quantity, decimal UnitPrice, int TaxRate, decimal TaxAmount, decimal SubTotalPrice, decimal TotalPrice);

/// <summary>Mirrors POST /api/orders/checkout's response shape. PaymentClientSecret is null once the order is already Paid.</summary>
public record CreateOrderResult(Guid OrderId, string? PaymentClientSecret, string OrderNumber);
