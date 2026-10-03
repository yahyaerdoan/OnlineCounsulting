namespace OnlineConsulting.Maui.Shared.Infrastructure.LiveUpdates;

/// <summary>Per-circuit (web) / per-app (MAUI) "data changed" bus. Raised by app resume, pull-to-refresh, navigation and
/// the live Api connection; screens subscribe to the topics they show and reload from the Api.</summary>
public sealed class DataChangeNotifier
{
    private static readonly TimeSpan RefreshAllCooldown = TimeSpan.FromSeconds(3);
    private static readonly TimeSpan SummaryCooldown = TimeSpan.FromSeconds(10);

    private readonly List<Subscription> _subscriptions = [];
    private readonly Lock _gate = new();
    private DateTimeOffset _lastRefreshAll = DateTimeOffset.MinValue;
    private DateTimeOffset _lastSummary = DateTimeOffset.MinValue;

    /// <summary>Calls <paramref name="reload"/> when any of <paramref name="topics"/> (or <see cref="DataTopic.All"/>) changes; dispose to stop.</summary>
    public IDisposable Subscribe(Func<Task> reload, params DataTopic[] topics)
    {
        var subscription = new Subscription(this, reload, [.. topics]);
        lock (_gate)
        {
            _subscriptions.Add(subscription);
        }

        return subscription;
    }

    /// <summary>Tells subscribers of <paramref name="topic"/> to reload. All and Summary are rate-limited so resume, pull and
    /// navigation firing together don't reload the same screen several times.</summary>
    public async Task NotifyAsync(DataTopic topic)
    {
        if (!PassesCooldown(topic))
        {
            return;
        }

        Subscription[] targets;
        lock (_gate)
        {
            targets = [.. _subscriptions.Where(s => topic == DataTopic.All || s.Topics.Contains(topic))];
        }

        foreach (var target in targets)
        {
            try
            {
                await target.Reload();
            }
            catch (ObjectDisposedException)
            {
            }
        }
    }

    private bool PassesCooldown(DataTopic topic)
    {
        var now = DateTimeOffset.UtcNow;
        lock (_gate)
        {
            switch (topic)
            {
                case DataTopic.All when now - _lastRefreshAll < RefreshAllCooldown:
                case DataTopic.Summary when now - _lastSummary < SummaryCooldown:
                    return false;
                case DataTopic.All:
                    _lastRefreshAll = now;
                    _lastSummary = now;
                    return true;
                case DataTopic.Summary:
                    _lastSummary = now;
                    return true;
                default:
                    return true;
            }
        }
    }

    private void Remove(Subscription subscription)
    {
        lock (_gate)
        {
            _ = _subscriptions.Remove(subscription);
        }
    }

    private sealed class Subscription(DataChangeNotifier owner, Func<Task> reload, HashSet<DataTopic> topics) : IDisposable
    {
        public Func<Task> Reload { get; } = reload;

        public HashSet<DataTopic> Topics { get; } = topics;

        public void Dispose() => owner.Remove(this);
    }
}
