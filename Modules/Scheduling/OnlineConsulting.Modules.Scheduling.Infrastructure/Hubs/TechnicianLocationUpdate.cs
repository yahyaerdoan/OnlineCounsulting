namespace OnlineConsulting.Modules.Scheduling.Infrastructure.Hubs;

public record TechnicianLocationUpdate(Guid AppointmentId, double Latitude, double Longitude, DateTimeOffset Timestamp);
