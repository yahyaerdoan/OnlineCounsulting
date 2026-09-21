using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Order;

/// <summary>Orders have no hard-delete endpoint - RefundAsync is what the old "Delete" button now calls instead.</summary>
public interface IAdminOrderService
{
    /// <summary>Fetches all orders.</summary>
    Task<List<AdminOrderListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Refunds an order, fully by default or partially when an amount is given.</summary>
    Task<ApiEnvelope> RefundAsync(Guid orderId, decimal? amount = null, CancellationToken cancellationToken = default);
}
