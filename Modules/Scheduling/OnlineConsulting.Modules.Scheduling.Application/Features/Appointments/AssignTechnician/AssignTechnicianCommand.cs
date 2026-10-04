using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Scheduling.Application.Common;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Abstractions;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Rules;
using OnlineConsulting.Modules.Scheduling.Application.Features.TechnicianTracking.Abstractions;
using OnlineConsulting.Modules.Scheduling.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.AssignTechnician;

/// <summary>Dispatch - also what authorizes the technician to push live location updates via TechnicianTrackingHub.PushLocation.</summary>
public record AssignTechnicianCommand(Guid Id, Guid TechnicianUserId) : IRequest<OperationResult>, ISecureAddRequest
{
    public string[] Roles => [SchedulingOperationClaims.Admin, SchedulingOperationClaims.Write, SchedulingOperationClaims.Update];
}

public class AssignTechnicianHandler(IAppointmentRepository repository, ITechnicianTrackingHubService hubService, IAppointmentNotifier notifier) : IRequestHandler<AssignTechnicianCommand, OperationResult>
{
    public async Task<OperationResult> Handle(AssignTechnicianCommand request, CancellationToken cancellationToken)
    {
        var appointment = await repository.GetAsync(a => a.Id == request.Id, cancellationToken: cancellationToken);

        if (appointment is null)
        {
            return AppointmentBusinessRules.AppointmentNotFound(request.Id);
        }

        if (appointment.IsClosed)
        {
            return Result.Conflict(SchedulingMessages.CannotAssignTechnicianToClosedAppointment);
        }

        if (appointment.AssignedTechnicianUserId == request.TechnicianUserId)
        {
            return Result.Success("Technician is already assigned.");
        }

        var previousTechnicianUserId = appointment.AssignedTechnicianUserId;

        appointment.AssignTechnician(request.TechnicianUserId);

        _ = await repository.UpdateAsync(appointment, cancellationToken: cancellationToken);

        await hubService.NotifyTechnicianAssignedAsync(appointment.Id, request.TechnicianUserId, cancellationToken);

        await notifier.TechnicianAssignedAsync(appointment, previousTechnicianUserId, cancellationToken);

        return Result.Success("Technician assigned successfully.");
    }
}
