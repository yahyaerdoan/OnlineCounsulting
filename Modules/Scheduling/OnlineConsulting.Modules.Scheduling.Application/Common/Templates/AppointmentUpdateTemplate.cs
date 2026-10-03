using OnlineConsulting.SharedKernel.Notifications.Templates;
using System.Net;

namespace OnlineConsulting.Modules.Scheduling.Application.Common.Templates;

public enum AppointmentUpdateKind
{
    Requested,
    Confirmed,
    CancelledByCustomer,
    CancelledByStaff,
    TechnicianAssigned,
    Completed,
}

public record AppointmentUpdateEmailModel(AppointmentUpdateKind Kind, string CustomerFirstName, string ServiceTitle, DateTimeOffset ScheduledStart,
    DateTimeOffset ScheduledEnd, string? ServiceAddress = null, string? TechnicianName = null, string? Reason = null, bool IsOnline = false, string? Topic = null);

/// <summary>Every customer-facing appointment email: one layout (greeting, what happened, the visit's details, what's next) so the
/// customer sees the same card at each step from request to completion.</summary>
public class AppointmentUpdateTemplate : IEmailTemplate<AppointmentUpdateEmailModel>
{
    public string Subject(AppointmentUpdateEmailModel model) => model.Kind switch
    {
        AppointmentUpdateKind.Requested => "We received your booking request",
        AppointmentUpdateKind.Confirmed => $"Confirmed: your visit on {AppointmentTimeText.Day(model.ScheduledStart)}",
        AppointmentUpdateKind.CancelledByCustomer => "Your appointment has been cancelled",
        AppointmentUpdateKind.CancelledByStaff => "Your appointment was cancelled",
        AppointmentUpdateKind.TechnicianAssigned => "Your technician has been assigned",
        AppointmentUpdateKind.Completed => "Your service is complete",
        _ => "Appointment update",
    };

    public string Build(AppointmentUpdateEmailModel model)
    {
        var (intro, next) = model.Kind switch
        {
            AppointmentUpdateKind.Requested => ("Thanks for booking with us! We received your request.", "We'll review it and let you know as soon as it's confirmed."),
            AppointmentUpdateKind.Confirmed when model.IsOnline => ("Good news: your online meeting is confirmed.", "We'll send you the details to join the call before it starts. Need to change something? You can cancel from your account."),
            AppointmentUpdateKind.Confirmed => ("Good news: your visit is confirmed.", "We'll let you know when a technician is assigned. Need to change something? You can cancel from your account."),
            AppointmentUpdateKind.CancelledByCustomer => ("As requested, we've cancelled your appointment.", "Changed your mind? You can book a new time any time from your account."),
            AppointmentUpdateKind.CancelledByStaff => ("We're sorry, but we had to cancel your appointment.", "Please book a new time from your account or reply to this email and we'll help you reschedule."),
            AppointmentUpdateKind.TechnicianAssigned => ("A technician has been assigned to your visit.", "You'll be able to follow them live in the app on the day of your visit."),
            AppointmentUpdateKind.Completed => ("Your service visit is complete. Thanks for choosing us!", "You can find the work report and your equipment history in your account."),
            _ => ("There's an update on your appointment.", ""),
        };

        var rows = new List<(string Label, string Value)>
        {
            ("Service", model.ServiceTitle),
            ("When", AppointmentTimeText.Range(model.ScheduledStart, model.ScheduledEnd)),
        };

        if (model.IsOnline)
        {
            rows.Add(("Meeting", "Online video call"));
            if (!string.IsNullOrWhiteSpace(model.Topic))
            {
                rows.Add(("Topic", model.Topic));
            }
        }
        else if (!string.IsNullOrWhiteSpace(model.ServiceAddress))
        {
            rows.Add(("Address", model.ServiceAddress));
        }

        if (!string.IsNullOrWhiteSpace(model.TechnicianName))
        {
            rows.Add(("Technician", model.TechnicianName));
        }

        if (!string.IsNullOrWhiteSpace(model.Reason))
        {
            rows.Add(("Reason", model.Reason));
        }

        var details = string.Concat(rows.Select(row => $"""
            <tr>
                <td style="padding: 6px 12px 6px 0; color: #777777; vertical-align: top;">{WebUtility.HtmlEncode(row.Label)}</td>
                <td style="padding: 6px 0; font-weight: bold;">{WebUtility.HtmlEncode(row.Value)}</td>
            </tr>
            """));

        var greeting = string.IsNullOrWhiteSpace(model.CustomerFirstName) ? "Hi," : $"Hi {WebUtility.HtmlEncode(model.CustomerFirstName)},";

        return EmailLayout.Wrap($"""
            <p>{greeting}</p>
            <p>{WebUtility.HtmlEncode(intro)}</p>
            <table style="border-collapse: collapse; margin: 16px 0;">{details}</table>
            <p>{WebUtility.HtmlEncode(next)}</p>
            """);
    }
}
