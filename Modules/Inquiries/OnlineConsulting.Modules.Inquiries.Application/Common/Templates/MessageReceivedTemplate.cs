using System.Net;
using OnlineConsulting.SharedKernel.Notifications.Templates;

namespace OnlineConsulting.Modules.Inquiries.Application.Common.Templates;

public record MessageReceivedEmailModel(string FirstName, string Subject);

/// <summary>Confirmation sent to whoever submitted a contact form.</summary>
public class MessageReceivedTemplate : IEmailTemplate<MessageReceivedEmailModel>
{
    public string Subject(MessageReceivedEmailModel model) => "We received your message";

    public string Build(MessageReceivedEmailModel model) => EmailLayout.Wrap($"""
        <p>Hi {WebUtility.HtmlEncode(model.FirstName)},</p>
        <p>Thanks for reaching out - we received your message about "{WebUtility.HtmlEncode(model.Subject)}" and will get back to you shortly.</p>
        """);
}
