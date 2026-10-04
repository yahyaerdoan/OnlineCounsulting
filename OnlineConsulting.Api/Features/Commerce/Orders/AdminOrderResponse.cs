using Hateoas;

namespace OnlineConsulting.Api.Features.Commerce.Orders;

/// <summary>An order in the staff list, with the owner's email and user name from Identity.</summary>
public record AdminOrderResponse(Guid Id, string OrderNumber, string OrderStatus, string PaymentStatus, decimal TotalPrice, DateTimeOffset CreatedDate, Guid UserId, string? UserEmail, string? UserName) : LinkedRecord;
