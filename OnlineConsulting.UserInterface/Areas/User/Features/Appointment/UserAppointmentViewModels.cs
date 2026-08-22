namespace OnlineConsulting.UserInterface.Areas.User.Features.Appointment;

public record UserAppointmentListItemViewModel(
    Guid Id,
    string ServiceName,
    DateTimeOffset ScheduledStart,
    DateTimeOffset ScheduledEnd,
    string Status,
    string? CustomerNote,
    string? ServiceAddress,
    string? NavigationUrl,
    bool CanCancel);
