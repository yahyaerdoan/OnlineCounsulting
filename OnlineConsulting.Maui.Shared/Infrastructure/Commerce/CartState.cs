using OnlineConsulting.Maui.Shared.Infrastructure.Api;
using OnlineConsulting.Maui.Shared.Infrastructure.LiveUpdates;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Commerce;

/// <summary>Shares the basket item count across layout/pages so adding to cart updates the badge live; reloads itself
/// when the basket changes elsewhere (another device, app resume, navigation).</summary>
public sealed class CartState : IDisposable
{
    private readonly IApiClient _apiClient;
    private readonly IDisposable _subscription;
    private Task? _loadTask;

    public CartState(IApiClient apiClient, DataChangeNotifier notifier)
    {
        _apiClient = apiClient;
        _subscription = notifier.Subscribe(ReloadAsync, DataTopic.Basket, DataTopic.Summary);
    }

    public int Count { get; private set; }

    public event Action? Changed;

    public void SetCount(int count)
    {
        Count = count;
        Changed?.Invoke();
    }

    /// <summary>Loads the count once per circuit - safe to call from every badge instance on the page.</summary>
    public Task EnsureLoadedAsync() => _loadTask ??= ReloadAsync();

    /// <summary>Fetches the current count from the Api; a failed call keeps the last known count.</summary>
    public async Task ReloadAsync()
    {
        var result = await _apiClient.GetAsync<int>(ApiRoutes.Commerce.Baskets.Count);
        if (result.IsSuccessful)
        {
            SetCount(result.ResultData);
        }
    }

    public void Dispose() => _subscription.Dispose();
}
