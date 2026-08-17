using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Order;

public class AdminOrderService(IApiClient apiClient) : IAdminOrderService
{
    private const string OrdersPath = "/api/orders";

    public async Task<List<AdminOrderListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<List<AdminOrderResponse>>($"{OrdersPath}/admin", cancellationToken);
        return (result.ResultData ?? []).Select(o => new AdminOrderListItemViewModel(
            o.Id, o.OrderNumber, o.OrderStatus, o.PaymentStatus, o.TotalPrice, o.CreatedDate,
            o.UserName ?? o.UserEmail ?? o.UserId.ToString(), o.UserEmail)).ToList();
    }

    public Task<ApiEnvelope> RefundAsync(Guid orderId, decimal? amount = null, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync($"{OrdersPath}/{orderId}/refund", new { OrderId = orderId, Amount = amount }, cancellationToken);
}
