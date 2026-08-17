using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Order;

/// <summary>Admin-wide order list/refund via the Api's admin-orders endpoint. There is no hard-delete endpoint
/// for orders - RefundAsync is what the admin list's old "Delete" button now calls, since refunding is the real
/// world equivalent of taking back a placed order.</summary>
public interface IAdminOrderService
{
    Task<List<AdminOrderListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ApiEnvelope> RefundAsync(Guid orderId, decimal? amount = null, CancellationToken cancellationToken = default);
}
