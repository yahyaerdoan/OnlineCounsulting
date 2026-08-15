using OnlineConsulting.SharedKernel.Notifications.Templates;
using System.Net;

namespace OnlineConsulting.Modules.Inquiries.Application.Common.Templates;

public record NewsletterSubscribedEmailModel(string Email);

/// <summary>Confirmation sent when a newsletter subscription is created.</summary>
public class NewsletterSubscribedTemplate : IEmailTemplate<NewsletterSubscribedEmailModel>
{
    public string Subject(NewsletterSubscribedEmailModel model) => "You're subscribed!";

    public string Build(NewsletterSubscribedEmailModel model) => EmailLayout.Wrap($"""
        <p>Thanks for subscribing to our newsletter with {WebUtility.HtmlEncode(model.Email)}. You'll hear from us soon.</p>
        """);
}
