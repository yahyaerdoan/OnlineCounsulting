using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Scheduling.Application.Common;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Abstractions;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Rules;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.CancelAppointmentByStaff;

/// <summary>The business cancelling a customer's visit (sick technician, weather, double booking); the optional reason is shown to the customer.</summary>
public record CancelAppointmentByStaffCommand(Guid Id, string? Reason) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SchedulingOperationClaims.Admin, SchedulingOperationClaims.Write, SchedulingOperationClaims.Update];
}

public class CancelAppointmentByStaffHandler(IAppointmentRepository repository, IAppointmentNotifier notifier) : IRequestHandler<CancelAppointmentByStaffCommand, OperationResult>
{
    public async Task<OperationResult> Handle(CancelAppointmentByStaffCommand request, CancellationToken cancellationToken)
    {
        var appointment = await repository.GetAsync(a => a.Id == request.Id, cancellationToken: cancellationToken);

        if (appointment is null)
        {
            return AppointmentBusinessRules.AppointmentNotFound(request.Id);
        }

        if (!appointment.CanBeCancelled)
        {
            return Result.Conflict(SchedulingMessages.OnlyPendingOrConfirmedCanBeCancelled);
        }

        appointment.Cancel();

        _ = await repository.UpdateAsync(appointment, cancellationToken: cancellationToken);

        await notifier.CancelledByStaffAsync(appointment, request.Reason, cancellationToken);

        return Result.Success("Appointment cancelled. The customer has been notified.");
    }
}
