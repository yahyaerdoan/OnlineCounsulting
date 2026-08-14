namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;

public record OrderDetailResponse(OrderResponse Order, IReadOnlyList<OrderItemResponse> Items, Guid ShippingAddressId, Guid InvoiceAddressId);
