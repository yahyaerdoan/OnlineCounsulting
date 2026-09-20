namespace OnlineConsulting.Maui.Shared.Infrastructure.Commerce;

/// <summary>Shares the basket item count across layout/pages so adding to cart updates the badge live.</summary>
public class CartState
{
    public int Count { get; private set; }

    public event Action? Changed;

    private Task? _loadTask;

    public void SetCount(int count)
    {
        Count = count;
        Changed?.Invoke();
    }

    /// <summary>Runs loader once per circuit - safe to call from every badge instance on the page.</summary>
    public Task EnsureLoadedAsync(Func<Task<int>> loader) => _loadTask ??= LoadAsync(loader);

    private async Task LoadAsync(Func<Task<int>> loader) => SetCount(await loader());
}
