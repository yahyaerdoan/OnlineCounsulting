using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Tenancy.Tests.Tenants;

public class BusinessTimeZoneTests
{
    private static readonly TimeZoneInfo Chicago = BusinessTimeZones.FindOrDefault("America/Chicago");

    [Fact]
    public void ToUtc_InWinter_UsesStandardOffset() =>
        Assert.Equal(new DateTimeOffset(2026, 1, 15, 14, 0, 0, TimeSpan.Zero), BusinessTimeZones.ToUtc(new DateOnly(2026, 1, 15), TimeSpan.FromHours(8), Chicago));

    [Fact]
    public void ToUtc_InSummer_UsesDaylightOffset() =>
        Assert.Equal(new DateTimeOffset(2026, 7, 15, 13, 0, 0, TimeSpan.Zero), BusinessTimeZones.ToUtc(new DateOnly(2026, 7, 15), TimeSpan.FromHours(8), Chicago));

    [Fact]
    public void ToUtc_TimeSkippedBySpringForward_IsNull() =>
        Assert.Null(BusinessTimeZones.ToUtc(new DateOnly(2026, 3, 8), new TimeSpan(2, 30, 0), Chicago));

    [Fact]
    public void ToUtc_TimeRepeatedByFallBack_UsesStandardOffset() =>
        Assert.Equal(new DateTimeOffset(2026, 11, 1, 7, 30, 0, TimeSpan.Zero), BusinessTimeZones.ToUtc(new DateOnly(2026, 11, 1), new TimeSpan(1, 30, 0), Chicago));

    [Fact]
    public void ToUtc_ReturnsUtcOffset() =>
        Assert.Equal(TimeSpan.Zero, BusinessTimeZones.ToUtc(new DateOnly(2026, 7, 15), TimeSpan.FromHours(8), Chicago)?.Offset);

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Mars/Olympus_Mons")]
    public void FindOrDefault_UnknownId_FallsBackToDefault(string? timeZoneId)
    {
        Assert.False(BusinessTimeZones.IsKnown(timeZoneId));
        Assert.Equal(BusinessTimeZones.FindOrDefault(BusinessTimeZones.Default).BaseUtcOffset, BusinessTimeZones.FindOrDefault(timeZoneId).BaseUtcOffset);
    }

    [Fact]
    public void FindOrDefault_KeepsTheIanaId() =>
        Assert.Equal("America/New_York", BusinessTimeZones.FindOrDefault("America/New_York").Id);

    [Fact]
    public void Tenant_StartsInDefaultZone_AndChangesToKnownZone()
    {
        var tenant = Tenant.Reserve("Acme HVAC", "acme-hvac", "owner@acme.test");
        Assert.Equal(BusinessTimeZones.Default, tenant.TimeZoneId);

        tenant.ChangeTimeZone("America/Denver");

        Assert.Equal("America/Denver", tenant.TimeZoneId);
    }

    [Fact]
    public void Tenant_ChangeTimeZone_RejectsUnknownZone() =>
        Assert.Throws<ArgumentException>(() => Tenant.Reserve("Acme HVAC", "acme-hvac", "owner@acme.test").ChangeTimeZone("Mars/Olympus_Mons"));
}
