using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Scheduling.Application.Common;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Abstractions;
using OnlineConsulting.Modules.Scheduling.Domain;
using OnlineConsulting.SharedKernel.Transactions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using OnlineConsulting.SharedKernel.Catalog;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.CreateAppointment;

/// <summary>ServiceId null means a generic meeting request, not a booking; ISchedulingTransactionRequest keeps the appointment write and its confirmation-email outbox row atomic.</summary>
public record CreateAppointmentCommand(Guid UserId, Guid? ServiceId, DateTimeOffset ScheduledStart, DateTimeOffset ScheduledEnd, string? CustomerNote, string? ServiceAddress = null,
    string MeetingType = AppointmentMeetingTypes.InPerson, string? Topic = null)
    : IRequest<OperationDataResult<Guid>>, ISecureAddRequest, ISchedulingTransactionRequest
{
    public string[] Roles => [];
}

/// <summary>A chosen service must be a Booking service - Products are bought through the basket, not scheduled.</summary>
public class CreateAppointmentHandler(IAppointmentRepository repository, IServiceCatalogReader catalogReader, IAppointmentNotifier notifier) : IRequestHandler<CreateAppointmentCommand, OperationDataResult<Guid>>
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

        var appointment = Appointment.Request(request.UserId, request.ServiceId, request.ScheduledStart, request.ScheduledEnd, request.MeetingType, request.Topic, request.ServiceAddress, request.CustomerNote);

        _ = await repository.AddAsync(appointment, cancellationToken: cancellationToken);

        await notifier.RequestedAsync(appointment, cancellationToken);

        return Result.Created(appointment.Id, "Appointment requested successfully.");
    }
}
