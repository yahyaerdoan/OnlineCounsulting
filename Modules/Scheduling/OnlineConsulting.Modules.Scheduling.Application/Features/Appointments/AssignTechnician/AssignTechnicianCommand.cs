using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Scheduling.Application.Common;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Abstractions;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Constants;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Rules;
using OnlineConsulting.Modules.Scheduling.Application.Features.TechnicianTracking.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.AssignTechnician;

/// <summary>Dispatch - assigns which technician is headed to this appointment, ahead of the visit. This is what authorizes that technician to push live location updates for the appointment (see TechnicianTrackingHub.PushLocation) and lets the customer's client start listening for them.</summary>
public record AssignTechnicianCommand(Guid Id, Guid TechnicianUserId) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SchedulingOperationClaims.Admin, SchedulingOperationClaims.Write, SchedulingOperationClaims.Update];
}

public class AssignTechnicianHandler(IAppointmentRepository repository, ITechnicianTrackingHubService hubService)
    : IRequestHandler<AssignTechnicianCommand, OperationResult>
{
    public async Task<OperationResult> Handle(AssignTechnicianCommand request, CancellationToken cancellationToken)
    {
        var appointment = await repository.GetAsync(a => a.Id == request.Id, cancellationToken: cancellationToken);
        if (appointment is null)
        {
            return AppointmentBusinessRules.AppointmentNotFound(request.Id);
        }

        if (appointment.Status is AppointmentStatuses.Cancelled or AppointmentStatuses.Completed)
        {
            return Result.BadRequest(SchedulingMessages.CannotAssignTechnicianToClosedAppointment);
        }

        appointment.AssignedTechnicianUserId = request.TechnicianUserId;
        _ = await repository.UpdateAsync(appointment);

        await hubService.NotifyTechnicianAssignedAsync(appointment.Id, appointment.UserId, request.TechnicianUserId, cancellationToken);

        return Result.Success("Technician assigned successfully.");
    }
}
