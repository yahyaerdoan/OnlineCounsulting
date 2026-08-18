using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Scheduling.Application.Common;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Abstractions;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Constants;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Rules;
using OnlineConsulting.SharedKernel.Notifications;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.CancelAppointment;

/// <summary>Self-service cancel - the caller can only cancel their own appointment, enforced by the UserId check in the handler rather than trusting a client-supplied owner id.</summary>
public record CancelAppointmentCommand(Guid Id, Guid UserId) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [];
}

public class CancelAppointmentHandler(IAppointmentRepository repository, IPushNotificationSender pushNotificationSender) : IRequestHandler<CancelAppointmentCommand, OperationResult>
{
    public async Task<OperationResult> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await repository.GetAsync(a => a.Id == request.Id && a.UserId == request.UserId, cancellationToken: cancellationToken);
        if (appointment is null)
            return AppointmentBusinessRules.AppointmentNotFound(request.Id);

        if (appointment.Status is not (AppointmentStatuses.Pending or AppointmentStatuses.Confirmed))
            return Result.BadRequest(SchedulingMessages.OnlyPendingOrConfirmedCanBeCancelled);

        appointment.Status = AppointmentStatuses.Cancelled;
        await repository.UpdateAsync(appointment);

        if (appointment.AssignedTechnicianUserId is { } technicianUserId)
        {
            await pushNotificationSender.SendToUserAsync(
                technicianUserId,
                "Appointment cancelled",
                "A customer has cancelled an appointment that was assigned to you.",
                new Dictionary<string, string> { ["appointmentId"] = appointment.Id.ToString() },
                cancellationToken);
        }

        return Result.Success("Appointment cancelled successfully.");
    }
}
