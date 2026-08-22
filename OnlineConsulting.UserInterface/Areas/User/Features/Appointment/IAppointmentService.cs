using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.User.Features.Appointment;

/// <summary>Raw Api calls for the customer's own appointments (/api/appointments/mine, /{id}/cancel) - scoped
/// server-side to the caller, never the admin-wide /api/appointments/admin endpoint.</summary>
public interface IAppointmentService
{
    Task<List<AppointmentResponse>> GetMineAsync(CancellationToken cancellationToken = default);
    Task<ApiEnvelope> CancelAsync(Guid id, CancellationToken cancellationToken = default);
}

public record AppointmentResponse(
    Guid Id,
    Guid UserId,
    Guid? ServiceId,
    DateTimeOffset ScheduledStart,
    DateTimeOffset ScheduledEnd,
    string Status,
    string? CustomerNote,
    bool RequiresPrepayment,
    Guid? AssignedTechnicianUserId,
    string? ServiceAddress,
    string? NavigationUrl);
