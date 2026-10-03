using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Scheduling.Application.Common;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Abstractions;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Constants;
using OnlineConsulting.Modules.Scheduling.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;
using OnlineConsulting.SharedKernel.Catalog;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.CreateAppointment;

/// <summary>ServiceId null means a generic meeting request, not a booking; ITransactionAddRequest keeps the appointment write and its confirmation-email outbox row atomic.</summary>
public record CreateAppointmentCommand(Guid UserId, Guid? ServiceId, DateTimeOffset ScheduledStart, DateTimeOffset ScheduledEnd, string? CustomerNote, string? ServiceAddress = null,
    string MeetingType = AppointmentMeetingTypes.InPerson, string? Topic = null)
    : IRequest<OperationDataResult<Guid>>, ISecureAddRequest, ITransactionAddRequest
{
    [JsonIgnore]
    public string[] Roles => [];
}

/// <summary>A chosen service must be a Booking service - Products are bought through the basket, not scheduled.</summary>
public class CreateAppointmentHandler(IAppointmentRepository repository, IServiceCatalogReader catalogReader, IAppointmentNotifier notifier)
    : IRequestHandler<CreateAppointmentCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        if (request.ScheduledEnd <= request.ScheduledStart)
        {
            return Result.UnprocessableContent<Guid>(SchedulingMessages.InvalidTimeRange);
        }

        if (request.ServiceId is Guid serviceId)
        {
            var catalogEntry = await catalogReader.GetAsync(serviceId, cancellationToken);
            if (catalogEntry is null)
            {
                return Result.NotFound<Guid>(SchedulingMessages.ServiceNotFound);
            }

            if (catalogEntry.Kind != ServiceKinds.Booking)
            {
                return Result.BadRequest<Guid>(SchedulingMessages.ServiceIsBoughtNotBooked);
            }
        }

        var overlaps = await repository.AnyAsync(a => a.Status != AppointmentStatuses.Cancelled && a.ScheduledStart < request.ScheduledEnd && a.ScheduledEnd > request.ScheduledStart, cancellationToken: cancellationToken);

        if (overlaps)
        {
            return Result.Conflict<Guid>(SchedulingMessages.SlotNoLongerAvailable);
        }

        var appointment = new Appointment
        {
            UserId = request.UserId,
            ServiceId = request.ServiceId,
            ScheduledStart = request.ScheduledStart,
            ScheduledEnd = request.ScheduledEnd,
            Status = AppointmentStatuses.Pending,
            CustomerNote = request.CustomerNote,
            MeetingType = request.MeetingType,
            Topic = request.MeetingType == AppointmentMeetingTypes.Online ? request.Topic?.Trim() : null,
            ServiceAddress = request.MeetingType == AppointmentMeetingTypes.InPerson ? request.ServiceAddress?.Trim() : null,
            RequiresPrepayment = false,
        };

        _ = await repository.AddAsync(appointment);

        await notifier.RequestedAsync(appointment, cancellationToken);

        return Result.Created(appointment.Id, "Appointment requested successfully.");
    }
}
