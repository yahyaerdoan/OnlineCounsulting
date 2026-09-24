namespace OnlineConsulting.Modules.Scheduling.Application.Features.TechnicianTracking.Abstractions;

/// <summary>Port for pushing tracking notifications into a SignalR group from outside the hub (e.g. a MediatR handler); implemented via IHubContext.</summary>
public interface ITechnicianTrackingHubService
{
    Task NotifyTechnicianAssignedAsync(Guid appointmentId, Guid customerUserId, Guid technicianUserId, CancellationToken cancellationToken = default);
}
