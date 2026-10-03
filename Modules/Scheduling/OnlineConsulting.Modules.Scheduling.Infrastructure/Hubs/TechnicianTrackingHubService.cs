using Microsoft.AspNetCore.SignalR;
using OnlineConsulting.Modules.Scheduling.Application.Features.TechnicianTracking.Abstractions;

namespace OnlineConsulting.Modules.Scheduling.Infrastructure.Hubs;

/// <summary>Updates an open tracking screen in real time; the push/email side of an assignment is IAppointmentNotifier's job.</summary>
public class TechnicianTrackingHubService(IHubContext<TechnicianTrackingHub, ITechnicianTrackingClient> hubContext) : ITechnicianTrackingHubService
{
    public Task NotifyTechnicianAssignedAsync(Guid appointmentId, Guid technicianUserId, CancellationToken cancellationToken = default) =>
        hubContext.Clients.Group(TechnicianTrackingHub.GroupName(appointmentId))
            .ReceivedTechnicianAssigned(new TechnicianAssignedUpdate(appointmentId, technicianUserId));
}
