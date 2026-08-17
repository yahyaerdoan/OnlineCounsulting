using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.User.Features.Order;

public class OrderService(IApiClient apiClient) : IOrderService
{
    private const string OrdersPath = "/api/orders";

    public async Task<List<OrderResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<List<OrderResponse>>(OrdersPath, cancellationToken);
        return result.ResultData ?? [];
    }

    public async Task<OrderDetailResponse?> GetDetailAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<OrderDetailResponse>($"{OrdersPath}/{id}", cancellationToken);
        return result.ResultData;
    }

    public async Task<OrderStatsResponse?> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<OrderStatsResponse>($"{OrdersPath}/stats", cancellationToken);
        return result.ResultData;
    }
}
