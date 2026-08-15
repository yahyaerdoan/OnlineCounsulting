using Hateoas;
using OnlineConsulting.Modules.Scheduling.Domain;

namespace OnlineConsulting.Modules.Scheduling.Application.Contracts;

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

    public static AppointmentResponse FromDomain(Appointment appointment) => new()
    {
        Id = appointment.Id,
        UserId = appointment.UserId,
        ServiceId = appointment.ServiceId,
        ScheduledStart = appointment.ScheduledStart,
        ScheduledEnd = appointment.ScheduledEnd,
        Status = appointment.Status,
        CustomerNote = appointment.CustomerNote,
        RequiresPrepayment = appointment.RequiresPrepayment,
    };
}
