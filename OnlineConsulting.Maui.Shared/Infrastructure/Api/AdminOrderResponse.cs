namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/orders/admin/query's response shape.</summary>
public record AdminOrderResponse(Guid Id, string OrderNumber, string OrderStatus, string PaymentStatus, decimal TotalPrice, DateTimeOffset CreatedDate, Guid UserId, string? UserEmail, string? UserName) : IQueryableFields
{
    public static string[] SearchFields => [nameof(OrderNumber), nameof(UserEmail), nameof(UserName)];
}
