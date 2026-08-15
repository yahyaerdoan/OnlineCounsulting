using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Scheduling.Domain;

/// <summary>A recurring weekly working-hours window the tenant is bookable in. Slots are computed on demand from these rules minus existing Appointments, never materialized into their own rows.</summary>
public class AvailabilityRule : TenantEntity<Guid>
{
    public required DayOfWeek DayOfWeek { get; set; }
    public required TimeSpan StartTime { get; set; }
    public required TimeSpan EndTime { get; set; }
    public required int SlotDurationMinutes { get; set; }
}
