using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Scheduling.Application.Common;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Abstractions;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Constants;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Rules;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.ConfirmAppointment;

/// <summary>Tenant/admin side approval of a Pending appointment. Payment is not involved yet (see Appointment.RequiresPrepayment) - confirmation is a manual decision until a real gateway exists.</summary>
public record ConfirmAppointmentCommand(Guid Id) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SchedulingOperationClaims.Admin, SchedulingOperationClaims.Write];
}

public class ConfirmAppointmentHandler(IAppointmentRepository repository) : IRequestHandler<ConfirmAppointmentCommand, OperationResult>
{
    public async Task<OperationResult> Handle(ConfirmAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await repository.GetAsync(a => a.Id == request.Id, cancellationToken: cancellationToken);
        if (appointment is null)
            return AppointmentBusinessRules.AppointmentNotFound(request.Id);

        if (appointment.Status != AppointmentStatuses.Pending)
            return Result.BadRequest(SchedulingMessages.OnlyPendingCanBeConfirmed);

        appointment.Status = AppointmentStatuses.Confirmed;
        await repository.UpdateAsync(appointment);

        return Result.Success("Appointment confirmed successfully.");
    }
}
