using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace OnlineConsulting.SharedKernel.Persistence;

public static class DateTimeOffsetConverters
{
    public static readonly ValueConverter<DateTimeOffset, DateTime> NonNullable = new(
        v => v.UtcDateTime,
        v => new DateTimeOffset(DateTime.SpecifyKind(v, DateTimeKind.Utc)));

    public static readonly ValueConverter<DateTimeOffset?, DateTime?> Nullable = new(
        v => v.HasValue ? v.Value.UtcDateTime : null,
        v => v.HasValue ? new DateTimeOffset(DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)) : null);
}
