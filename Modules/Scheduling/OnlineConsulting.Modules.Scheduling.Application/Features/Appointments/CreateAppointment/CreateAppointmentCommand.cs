using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
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

/// <summary>ServiceId null means a generic meeting request, not a booking; ITransactionAddRequest keeps the appointment write and its confirmation-email outbox row atomic.</summary>
public record CreateAppointmentCommand(Guid UserId, string Email, Guid? ServiceId, DateTimeOffset ScheduledStart, DateTimeOffset ScheduledEnd, string? CustomerNote, string? ServiceAddress = null)
    : IRequest<OperationDataResult<Guid>>, ISecureAddRequest, ITransactionAddRequest
{
    [JsonIgnore]
    public string[] Roles => [];
}

public class CreateAppointmentHandler(IAppointmentRepository repository, IEmailOutboxWriter<ISchedulingOutboxModule> outboxWriter, IEmailTemplate<AppointmentConfirmationEmailModel> confirmationTemplate)
    : IRequestHandler<CreateAppointmentCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        if (request.ScheduledEnd <= request.ScheduledStart)
        {
            return Result.BadRequest<Guid>(SchedulingMessages.InvalidTimeRange);
        }

        var overlaps = await repository.AnyAsync(a => a.Status != AppointmentStatuses.Cancelled && a.ScheduledStart < request.ScheduledEnd && a.ScheduledEnd > request.ScheduledStart, cancellationToken: cancellationToken);

        if (overlaps)
        {
            return Result.BadRequest<Guid>(SchedulingMessages.SlotNoLongerAvailable);
        }

        var appointment = new Appointment
        {
            UserId = request.UserId,
            ServiceId = request.ServiceId,
            ScheduledStart = request.ScheduledStart,
            ScheduledEnd = request.ScheduledEnd,
            Status = AppointmentStatuses.Pending,
            CustomerNote = request.CustomerNote,
            ServiceAddress = request.ServiceAddress,
            RequiresPrepayment = false,
        };

        _ = await repository.AddAsync(appointment);

        var confirmationModel = new AppointmentConfirmationEmailModel(appointment.ScheduledStart, appointment.ScheduledEnd, appointment.ServiceId is not null);

        await outboxWriter.EnqueueAsync(request.Email, confirmationTemplate.Subject(confirmationModel), confirmationTemplate.Build(confirmationModel), sourceReference: $"Appointment:{appointment.Id}", cancellationToken: cancellationToken);

        return Result.Created(appointment.Id, "Appointment requested successfully.");
    }
}
