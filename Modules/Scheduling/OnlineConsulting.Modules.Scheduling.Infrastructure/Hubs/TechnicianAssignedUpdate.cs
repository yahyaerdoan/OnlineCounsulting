namespace OnlineConsulting.Modules.Scheduling.Infrastructure.Hubs;

public record TechnicianAssignedUpdate(Guid AppointmentId, Guid TechnicianUserId);
