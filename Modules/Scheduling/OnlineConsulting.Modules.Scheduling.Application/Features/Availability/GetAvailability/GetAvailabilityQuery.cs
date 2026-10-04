using MediatR;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Abstractions;
using OnlineConsulting.Modules.Scheduling.Application.Features.Availability.Abstractions;
using OnlineConsulting.Modules.Scheduling.Application.Features.Availability.Contracts;
using OnlineConsulting.Modules.Scheduling.Domain;
using OnlineConsulting.SharedKernel.Persistence;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.Availability.GetAvailability;

/// <summary>Free slots on a day of the business's own calendar: availability rules are read in its time zone, slots come back in UTC,
/// and slots that are taken or already past are left out.</summary>
public record GetAvailabilityQuery(DateOnly Date) : IRequest<OperationDataResult<List<AvailableSlotResponse>>>;

public class GetAvailabilityHandler(IAvailabilityRuleRepository ruleRepository, IAppointmentRepository appointmentRepository, ITenantProvider tenantProvider,
    ITenantTimeZoneReader timeZoneReader) : IRequestHandler<GetAvailabilityQuery, OperationDataResult<List<AvailableSlotResponse>>>
{
    public async Task<OperationDataResult<List<AvailableSlotResponse>>> Handle(GetAvailabilityQuery request, CancellationToken cancellationToken)
    {
        var rules = await ruleRepository.GetListAsync(r => r.DayOfWeek == request.Date.DayOfWeek, orderBy: q => q.OrderBy(r => r.StartTime),
            size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);

        if (rules.Items.Count == 0)
        {
            return Result.Success(new List<AvailableSlotResponse>(), "No working hours configured for this day.");
        }

        var zone = await timeZoneReader.GetAsync(tenantProvider.TenantId, cancellationToken);
        var now = DateTimeOffset.UtcNow;
        var candidates = rules.Items.SelectMany(rule => SlotsFor(request.Date, rule, zone)).Where(slot => slot.Start > now).ToList();

        if (candidates.Count == 0)
        {
            return Result.Success(candidates, "No open slots left for this day.");
        }

        var windowStart = candidates.Min(s => s.Start);
        var windowEnd = candidates.Max(s => s.End);
        var existingAppointments = await appointmentRepository.GetListAsync(
            a => a.Status != AppointmentStatuses.Cancelled && a.ScheduledStart < windowEnd && a.ScheduledEnd > windowStart,
            orderBy: q => q.OrderBy(a => a.Id), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);

        var slots = candidates
            .Where(slot => !existingAppointments.Items.Any(a => a.ScheduledStart < slot.End && a.ScheduledEnd > slot.Start))
            .OrderBy(slot => slot.Start)
            .ToList();

        return Result.Success(slots, "Availability retrieved successfully.");
    }

    private static IEnumerable<AvailableSlotResponse> SlotsFor(DateOnly date, AvailabilityRule rule, TimeZoneInfo zone)
    {
        var duration = TimeSpan.FromMinutes(rule.SlotDurationMinutes);
        if (duration <= TimeSpan.Zero)
        {
            yield break;
        }

        for (var cursor = rule.StartTime; cursor + duration <= rule.EndTime; cursor += duration)
        {
            if (BusinessTimeZones.ToUtc(date, cursor, zone) is { } start && BusinessTimeZones.ToUtc(date, cursor + duration, zone) is { } end)
            {
                yield return new AvailableSlotResponse(start, end);
            }
        }
    }
}
