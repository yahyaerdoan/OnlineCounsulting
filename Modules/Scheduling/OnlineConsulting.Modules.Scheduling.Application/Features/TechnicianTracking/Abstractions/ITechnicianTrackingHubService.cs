namespace OnlineConsulting.Modules.Scheduling.Application.Features.TechnicianTracking.Abstractions;

/// <summary>Port for pushing tracking-related notifications to an appointment's SignalR group from outside the hub (e.g. from a MediatR handler after a REST write) - implemented in Infrastructure via IHubContext, same Dependency Inversion seam as any other outbound integration in this codebase.</summary>
public interface ITechnicianTrackingHubService
{
    Task NotifyTechnicianAssignedAsync(Guid appointmentId, Guid customerUserId, Guid technicianUserId, CancellationToken cancellationToken = default);
}
