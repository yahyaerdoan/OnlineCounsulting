using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Scheduling.Application.Common;
using OnlineConsulting.Modules.Scheduling.Application.Common.Templates;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Abstractions;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Constants;
using OnlineConsulting.Modules.Scheduling.Domain;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Notifications.Templates;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.CreateAppointment;

/// <summary>Books a service (ServiceId set) or requests a generic meeting with the tenant (ServiceId null) - same underlying Appointment either way. UserId is always resolved server-side from the authenticated caller, never trusted from the client. Single write (AddAsync only) - deliberately not ITransactionAddRequest, which is reserved for handlers with 2+ SaveChanges calls (see project rule: adding it to a single-SaveChanges handler causes an MSDTC error).</summary>
public record CreateAppointmentCommand(Guid UserId, string Email, Guid? ServiceId, DateTimeOffset ScheduledStart, DateTimeOffset ScheduledEnd, string? CustomerNote, string? ServiceAddress = null)
    : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [];
}

public class CreateAppointmentHandler(IAppointmentRepository repository, IEmailOutboxWriter outboxWriter, IEmailTemplate<AppointmentConfirmationEmailModel> confirmationTemplate)
    : IRequestHandler<CreateAppointmentCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        if (request.ScheduledEnd <= request.ScheduledStart)
            return Result.BadRequest<Guid>(SchedulingMessages.InvalidTimeRange);

        var overlaps = await repository.AnyAsync(a =>
            a.Status != AppointmentStatuses.Cancelled &&
            a.ScheduledStart < request.ScheduledEnd &&
            a.ScheduledEnd > request.ScheduledStart,
            cancellationToken: cancellationToken);

        if (overlaps)
            return Result.BadRequest<Guid>(SchedulingMessages.SlotNoLongerAvailable);

        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            ServiceId = request.ServiceId,
            ScheduledStart = request.ScheduledStart,
            ScheduledEnd = request.ScheduledEnd,
            Status = AppointmentStatuses.Pending,
            CustomerNote = request.CustomerNote,
            ServiceAddress = request.ServiceAddress,
            // Always false for now - no payment gateway exists yet, so no appointment can be gated on payment. Wiring this from Service.RequiresPrepayment is deferred until that integration lands (see AppointmentStatuses.PendingPayment).
            RequiresPrepayment = false,
        };

        // Staged before AddAsync (not saved yet - EnqueueEmail just tracks the row) so AddAsync's own
        // SaveChangesAsync flushes both in one round-trip; this handler isn't ITransactionAddRequest,
        // so there's no later write to piggyback on if the outbox row were staged afterward.
        var confirmationModel = new AppointmentConfirmationEmailModel(appointment.ScheduledStart, appointment.ScheduledEnd, appointment.ServiceId is not null);
        outboxWriter.Enqueue(request.Email, confirmationTemplate.Subject(confirmationModel), confirmationTemplate.Build(confirmationModel), sourceReference: $"Appointment:{appointment.Id}");

        await repository.AddAsync(appointment);

        return Result.Created(appointment.Id, "Appointment requested successfully.");
    }
}
