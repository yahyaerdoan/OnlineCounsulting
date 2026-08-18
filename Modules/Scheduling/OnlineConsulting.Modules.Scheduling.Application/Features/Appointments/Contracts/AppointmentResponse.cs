using Hateoas;
using OnlineConsulting.Modules.Scheduling.Application.Features.AppointmentMediaItems.Contracts;
using OnlineConsulting.Modules.Scheduling.Domain;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Contracts;

/// <summary>A class with required init properties instead of a positional record, since records can't inherit LinkedResponse.</summary>
public class AppointmentResponse : LinkedResponse
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }
    public Guid? ServiceId { get; init; }
    public required DateTimeOffset ScheduledStart { get; init; }
    public required DateTimeOffset ScheduledEnd { get; init; }
    public required string Status { get; init; }
    public string? CustomerNote { get; init; }
    public required bool RequiresPrepayment { get; init; }
    public Guid? AssignedTechnicianUserId { get; init; }
    public string? ServiceAddress { get; init; }

    /// <summary>Google Maps turn-by-turn directions deep link to ServiceAddress (opens the device's own maps app - Google Maps, Waze, or Apple Maps all honor this URL scheme). Null when there's no address to navigate to. Built from free text, not coordinates - no geocoding/routing API involved, the maps app resolves the address itself.</summary>
    public string? NavigationUrl { get; init; }

    /// <summary>The customer's pre-diagnosis photo/video gallery - empty for list-view queries (GetMyAppointments) to avoid an N+1 join per row, populated only by GetAppointmentById.</summary>
    public List<AppointmentMediaItemResponse> MediaItems { get; init; } = [];

    public static AppointmentResponse FromDomain(Appointment appointment, List<AppointmentMediaItemResponse>? mediaItems = null) => new()
    {
        Id = appointment.Id,
        UserId = appointment.UserId,
        ServiceId = appointment.ServiceId,
        ScheduledStart = appointment.ScheduledStart,
        ScheduledEnd = appointment.ScheduledEnd,
        Status = appointment.Status,
        CustomerNote = appointment.CustomerNote,
        RequiresPrepayment = appointment.RequiresPrepayment,
        AssignedTechnicianUserId = appointment.AssignedTechnicianUserId,
        ServiceAddress = appointment.ServiceAddress,
        NavigationUrl = string.IsNullOrWhiteSpace(appointment.ServiceAddress)
            ? null
            : $"https://www.google.com/maps/dir/?api=1&destination={Uri.EscapeDataString(appointment.ServiceAddress)}",
        MediaItems = mediaItems ?? [],
    };
}
