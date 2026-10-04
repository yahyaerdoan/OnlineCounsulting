namespace OnlineConsulting.SharedKernel.Tenancy;

/// <summary>IANA time zones a business runs in. Times are stored in UTC and shown, and scheduled, in the business's zone.</summary>
public static class BusinessTimeZones
{
    public const string Default = "America/Chicago";

    public static bool IsKnown(string? timeZoneId) => Find(timeZoneId) is not null;

    /// <summary>The zone for the id, or the <see cref="Default"/> zone when the id is missing or unknown.</summary>
    public static TimeZoneInfo FindOrDefault(string? timeZoneId) => Find(timeZoneId) ?? TimeZoneInfo.FindSystemTimeZoneById(Default);

    /// <summary>The UTC instant of a wall-clock time in the zone; null when that time doesn't exist there (skipped by a daylight-saving change).</summary>
    public static DateTimeOffset? ToUtc(DateOnly date, TimeSpan timeOfDay, TimeZoneInfo zone)
    {
        var local = date.ToDateTime(TimeOnly.MinValue).Add(timeOfDay);
        return zone.IsInvalidTime(local) ? null : new DateTimeOffset(local, zone.GetUtcOffset(local)).ToUniversalTime();
    }

    /// <summary>The same instant on the business's wall clock, for formatting dates and times the customer reads.</summary>
    public static DateTimeOffset InZone(this DateTimeOffset value, TimeZoneInfo zone) => TimeZoneInfo.ConvertTime(value, zone);

    private static TimeZoneInfo? Find(string? timeZoneId) =>
        timeZoneId is { Length: > 0 } id && TimeZoneInfo.TryFindSystemTimeZoneById(id, out var zone) ? zone : null;
}
