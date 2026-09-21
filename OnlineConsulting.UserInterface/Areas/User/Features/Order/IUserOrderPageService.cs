namespace OnlineConsulting.UserInterface.Areas.User.Features.Order;

/// <summary>The dashboard's own-orders screens. Deliberately backed by /api/orders (GetOrders/GetOrderDetail/
/// GetOrderStats), which the Api scopes to the caller - never the admin-wide /api/orders/admin endpoint.</summary>
public interface IUserOrderPageService
{
    /// <summary>Gets the current user's orders, newest first.</summary>
    Task<List<UserOrderListItemViewModel>> GetMyOrdersAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets order detail with resolved addresses and per-item service/category info.</summary>
    Task<UserOrderDetailViewModel> GetMyOrderDetailAsync(Guid orderId, CancellationToken cancellationToken = default);

    /// <summary>Gets dashboard stats (totals, pending/paid/cancelled counts, spend) for the current user.</summary>
    Task<UserDashboardStatsViewModel> GetMyStatsAsync(CancellationToken cancellationToken = default);
}
