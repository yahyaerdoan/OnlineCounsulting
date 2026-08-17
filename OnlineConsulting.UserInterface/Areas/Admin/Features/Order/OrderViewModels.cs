namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Order;

/// <summary>Wire shape of GET /api/orders/admin - UserEmail/UserName are joined in Api-side from Identity.</summary>
public record AdminOrderResponse(Guid Id, string OrderNumber, string OrderStatus, string PaymentStatus, decimal TotalPrice, DateTimeOffset CreatedDate, Guid UserId, string? UserEmail, string? UserName);

public record AdminOrderListItemViewModel(
    Guid Id,
    string OrderNumber,
    string OrderStatus,
    string PaymentStatus,
    decimal TotalPrice,
    DateTimeOffset CreatedDate,
    string OwnerDisplayName,
    string? UserEmail);
