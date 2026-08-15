using System.Net;
using OnlineConsulting.SharedKernel.Notifications.Templates;

namespace OnlineConsulting.Modules.Inquiries.Application.Common.Templates;

public record NewInquiryNotificationEmailModel(string FirstName, string LastName, string Email, string Subject, string Description);

// Notification sent to the admin inbox when a new contact form arrives.
public class NewInquiryNotificationTemplate : IEmailTemplate<NewInquiryNotificationEmailModel>
{
    public string Subject(NewInquiryNotificationEmailModel model) => $"New inquiry: {model.Subject}";

    public string Build(NewInquiryNotificationEmailModel model) => EmailLayout.Wrap($"""
        <p>From: {WebUtility.HtmlEncode(model.FirstName)} {WebUtility.HtmlEncode(model.LastName)} ({WebUtility.HtmlEncode(model.Email)})</p>
        <p>{WebUtility.HtmlEncode(model.Description)}</p>
        """);
}
