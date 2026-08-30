namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors GET /api/scheduling/availability-rules's response shape.</summary>
public record AvailabilityRuleResponse(Guid Id, DayOfWeek DayOfWeek, TimeSpan StartTime, TimeSpan EndTime, int SlotDurationMinutes);
