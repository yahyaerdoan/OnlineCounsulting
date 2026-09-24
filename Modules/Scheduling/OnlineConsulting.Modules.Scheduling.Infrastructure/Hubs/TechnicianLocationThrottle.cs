using System.Collections.Concurrent;

namespace OnlineConsulting.Modules.Scheduling.Infrastructure.Hubs;

/// <summary>Rate-limits PushLocation per connection so a buggy/malicious client can't flood the hub; in-memory only, needs Redis to work across instances.</summary>
internal static class TechnicianLocationThrottle
{
    private static readonly ConcurrentDictionary<string, DateTimeOffset> LastPushAt = new();
    private static readonly TimeSpan MinInterval = TimeSpan.FromSeconds(1);

    public static bool TryAcquire(string connectionId)
    {
        var now = DateTimeOffset.UtcNow;
        var last = LastPushAt.GetOrAdd(connectionId, DateTimeOffset.MinValue);
        if (now - last < MinInterval)
        {
            return false;
        }

        LastPushAt[connectionId] = now;
        return true;
    }

    public static void Release(string connectionId) => LastPushAt.TryRemove(connectionId, out _);
}
