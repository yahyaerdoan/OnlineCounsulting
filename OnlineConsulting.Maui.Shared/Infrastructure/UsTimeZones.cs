namespace OnlineConsulting.Maui.Shared.Infrastructure;

/// <summary>US time zones a business can run in, as IANA ids with the names customers know them by.</summary>
public static class UsTimeZones
{
    public static readonly IReadOnlyList<(string Id, string Name)> All =
    [
        ("America/New_York", "Eastern Time"),
        ("America/Chicago", "Central Time"),
        ("America/Denver", "Mountain Time"),
        ("America/Phoenix", "Mountain Time (Arizona, no daylight saving)"),
        ("America/Los_Angeles", "Pacific Time"),
        ("America/Anchorage", "Alaska Time"),
        ("Pacific/Honolulu", "Hawaii Time"),
    ];

    /// <summary>The listed zones, plus <paramref name="currentId"/> when it isn't one of them, so a saved zone always shows.</summary>
    public static IEnumerable<(string Id, string Name)> WithCurrent(string? currentId) =>
        currentId is { Length: > 0 } id && All.All(zone => zone.Id != id) ? [.. All, (id, id)] : All;
}
