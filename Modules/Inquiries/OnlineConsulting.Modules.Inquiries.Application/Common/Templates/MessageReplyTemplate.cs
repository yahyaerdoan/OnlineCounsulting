using OnlineConsulting.SharedKernel.Notifications.Templates;
using System.Net;

namespace OnlineConsulting.Modules.Inquiries.Application.Common.Templates;

public record MessageReplyEmailModel(string FirstName, string Subject, string ReplyBody);

/// <summary>Sent when an admin replies to a submitted contact-form message.</summary>
public class MessageReplyTemplate : IEmailTemplate<MessageReplyEmailModel>
{
    public string Subject(MessageReplyEmailModel model) => $"Re: {model.Subject}";

    public string Build(MessageReplyEmailModel model) => EmailLayout.Wrap($"""
        <p>Hi {WebUtility.HtmlEncode(model.FirstName)},</p>
        <p>{WebUtility.HtmlEncode(model.ReplyBody).Replace("\n", "<br/>")}</p>
        """);
}
