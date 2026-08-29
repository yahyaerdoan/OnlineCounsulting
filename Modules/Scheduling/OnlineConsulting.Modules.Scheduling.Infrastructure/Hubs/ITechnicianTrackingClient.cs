namespace OnlineConsulting.Modules.Scheduling.Infrastructure.Hubs;

/// <summary>Strongly typed client contract for <see cref="TechnicianTrackingHub"/>.</summary>
public interface ITechnicianTrackingClient
{
    Task ReceivedTechnicianLocation(TechnicianLocationUpdate update);
    Task ReceivedTechnicianAssigned(TechnicianAssignedUpdate update);
}

public record TechnicianLocationUpdate(Guid AppointmentId, double Latitude, double Longitude, DateTimeOffset Timestamp);

public record TechnicianAssignedUpdate(Guid AppointmentId, Guid TechnicianUserId);
