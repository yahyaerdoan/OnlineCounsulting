using OnlineConsulting.Modules.Scheduling.Domain;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.Availability.Contracts;

public class AvailabilityRuleResponse
{
    public required Guid Id { get; init; }
    public required DayOfWeek DayOfWeek { get; init; }
    public required TimeSpan StartTime { get; init; }
    public required TimeSpan EndTime { get; init; }
    public required int SlotDurationMinutes { get; init; }

    public static AvailabilityRuleResponse FromDomain(AvailabilityRule rule) => new()
    {
        Id = rule.Id,
        DayOfWeek = rule.DayOfWeek,
        StartTime = rule.StartTime,
        EndTime = rule.EndTime,
        SlotDurationMinutes = rule.SlotDurationMinutes,
    };
}
