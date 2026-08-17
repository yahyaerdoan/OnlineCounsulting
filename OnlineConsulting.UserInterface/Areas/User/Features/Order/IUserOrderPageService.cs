namespace OnlineConsulting.UserInterface.Areas.User.Features.Order;

/// <summary>The dashboard's own-orders screens. Deliberately backed by /api/orders (GetOrders/GetOrderDetail/
/// GetOrderStats), which the Api scopes to the caller - never the admin-wide /api/orders/admin endpoint.</summary>
public interface IUserOrderPageService
{
    Task<List<UserOrderListItemViewModel>> GetMyOrdersAsync(CancellationToken cancellationToken = default);
    Task<UserOrderDetailViewModel> GetMyOrderDetailAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<UserDashboardStatsViewModel> GetMyStatsAsync(CancellationToken cancellationToken = default);
}
