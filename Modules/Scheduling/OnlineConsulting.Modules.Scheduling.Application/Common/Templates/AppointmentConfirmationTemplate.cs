using System.Net;
using OnlineConsulting.SharedKernel.Notifications.Templates;

namespace OnlineConsulting.Modules.Scheduling.Application.Common.Templates;

public record AppointmentConfirmationEmailModel(DateTimeOffset ScheduledStart, DateTimeOffset ScheduledEnd, bool IsServiceBooking);

/// <summary>Booking confirmation sent right after an appointment is requested. The appointment starts life as Pending regardless of type, so this confirms the request was received, not that the tenant has approved it yet.</summary>
public class AppointmentConfirmationTemplate : IEmailTemplate<AppointmentConfirmationEmailModel>
{
    public string Subject(AppointmentConfirmationEmailModel model) =>
        model.IsServiceBooking ? "Your booking request was received" : "Your meeting request was received";

    public string Build(AppointmentConfirmationEmailModel model) => EmailLayout.Wrap($"""
        <p>{WebUtility.HtmlEncode(model.IsServiceBooking ? "Thanks for your booking request!" : "Thanks for your meeting request!")}</p>
        <p>Requested time: <strong>{model.ScheduledStart:f}</strong> - <strong>{model.ScheduledEnd:t}</strong></p>
        <p>We'll notify you once it's confirmed.</p>
        """);
}
