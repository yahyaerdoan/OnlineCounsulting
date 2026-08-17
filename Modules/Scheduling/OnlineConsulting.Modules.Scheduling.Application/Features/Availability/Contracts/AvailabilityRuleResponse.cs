using Hateoas;
using OnlineConsulting.Modules.Scheduling.Domain;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.Availability.Contracts;

/// <summary>A class with required init properties instead of a positional record, since records can't inherit LinkedResponse.</summary>
public class AvailabilityRuleResponse : LinkedResponse
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
