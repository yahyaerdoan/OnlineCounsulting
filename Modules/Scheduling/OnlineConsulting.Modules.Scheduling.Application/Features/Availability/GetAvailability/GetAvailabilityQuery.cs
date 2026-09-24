using MediatR;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Abstractions;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Constants;
using OnlineConsulting.Modules.Scheduling.Application.Features.Availability.Abstractions;
using OnlineConsulting.Modules.Scheduling.Application.Features.Availability.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.Availability.GetAvailability;

/// <summary>Computed on demand from AvailabilityRule minus existing Appointments - never materialized, so rule changes apply immediately.</summary>
/// <param name="Date">Combined with rule times as UTC - tenant timezone isn't modeled yet, so this must already be aligned to UTC.</param>
public record GetAvailabilityQuery(DateOnly Date) : IRequest<OperationDataResult<List<AvailableSlotResponse>>>;

public class GetAvailabilityHandler(IAvailabilityRuleRepository ruleRepository, IAppointmentRepository appointmentRepository)
    : IRequestHandler<GetAvailabilityQuery, OperationDataResult<List<AvailableSlotResponse>>>
{
    public async Task<OperationDataResult<List<AvailableSlotResponse>>> Handle(GetAvailabilityQuery request, CancellationToken cancellationToken)
    {
        var dayOfWeek = request.Date.DayOfWeek;
        var rules = await ruleRepository.GetListAsync(r => r.DayOfWeek == dayOfWeek, orderBy: q => q.OrderBy(r => r.Id), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);

        if (rules.Items.Count == 0)
        {
            return Result.Success(new List<AvailableSlotResponse>(), "No working hours configured for this day.");
        }

        var dayStart = new DateTimeOffset(request.Date.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
        var dayEnd = dayStart.AddDays(1);

        var existingAppointments = await appointmentRepository.GetListAsync(
            a => a.Status != AppointmentStatuses.Cancelled && a.ScheduledStart < dayEnd && a.ScheduledEnd > dayStart,
            orderBy: q => q.OrderBy(a => a.Id), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);

        var slots = new List<AvailableSlotResponse>();

        foreach (var rule in rules.Items)
        {
            var slotDuration = TimeSpan.FromMinutes(rule.SlotDurationMinutes);
            var cursor = dayStart + rule.StartTime;
            var ruleEnd = dayStart + rule.EndTime;

            while (cursor + slotDuration <= ruleEnd)
            {
                var slotEnd = cursor + slotDuration;
                var isTaken = existingAppointments.Items.Any(a => a.ScheduledStart < slotEnd && a.ScheduledEnd > cursor);

                if (!isTaken)
                {
                    slots.Add(new AvailableSlotResponse(cursor, slotEnd));
                }

                cursor = slotEnd;
            }
        }

        return Result.Success(slots, "Availability retrieved successfully.");
    }
}
