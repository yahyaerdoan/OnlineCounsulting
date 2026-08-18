using System.Collections.Concurrent;

namespace OnlineConsulting.Modules.Scheduling.Infrastructure.Hubs;

/// <summary>Per-connection minimum interval between PushLocation calls - a buggy or malicious technician client could otherwise flood the hub (and every connected customer) with GPS pings far faster than any UI could use. In-memory/single-instance, same scaling caveat as the SignalR Redis backplane itself (a multi-instance deployment would need this synchronized too, e.g. via Redis).</summary>
internal static class TechnicianLocationThrottle
{
    private static readonly ConcurrentDictionary<string, DateTimeOffset> LastPushAt = new();
    private static readonly TimeSpan MinInterval = TimeSpan.FromSeconds(1);

    public static bool TryAcquire(string connectionId)
    {
        var now = DateTimeOffset.UtcNow;
        var last = LastPushAt.GetOrAdd(connectionId, DateTimeOffset.MinValue);
        if (now - last < MinInterval)
            return false;

        LastPushAt[connectionId] = now;
        return true;
    }

    public static void Release(string connectionId) => LastPushAt.TryRemove(connectionId, out _);
}
