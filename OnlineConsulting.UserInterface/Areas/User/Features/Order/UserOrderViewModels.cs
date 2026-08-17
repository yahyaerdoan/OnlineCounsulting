namespace OnlineConsulting.UserInterface.Areas.User.Features.Order;

public record UserOrderListItemViewModel(Guid Id, string OrderNumber, string OrderStatus, string PaymentStatus, decimal TotalPrice, DateTimeOffset CreatedDate);

public record UserOrderItemViewModel(
    Guid Id,
    Guid ServiceId,
    string ServiceTitle,
    string? ServiceImageUrl,
    string CategoryTitle,
    int Quantity,
    decimal UnitPrice,
    int TaxRate,
    decimal TaxAmount,
    decimal SubTotalPrice,
    decimal TotalPrice);

public record UserOrderAddressViewModel(string AddressName, string? CompanyName, string Country, string AddressLine, string City, string State, string Zipcode);

/// <summary>Replaces the legacy ResultOrderDetailDto - the Api's GetOrderDetail only returns address ids, so the
/// addresses and the per-item service details are resolved here rather than in the view.</summary>
public class UserOrderDetailViewModel
{
    public UserOrderListItemViewModel? Order { get; set; }
    public List<UserOrderItemViewModel> OrderItems { get; set; } = [];
    public UserOrderAddressViewModel ShippingAddress { get; set; } = EmptyAddress;
    public UserOrderAddressViewModel InvoiceAddress { get; set; } = EmptyAddress;

    public decimal SubTotal => OrderItems.Sum(i => i.SubTotalPrice);
    public decimal TaxTotal => OrderItems.Sum(i => i.TaxAmount);

    private static UserOrderAddressViewModel EmptyAddress =>
        new(string.Empty, null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
}

public record UserDashboardStatsViewModel(int TotalOrders, int PendingOrders, int PaidOrders, int CancelledOrders, decimal TotalSpent);
