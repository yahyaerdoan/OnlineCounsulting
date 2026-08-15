using System.Net;
using OnlineConsulting.SharedKernel.Notifications.Templates;

namespace OnlineConsulting.Modules.Inquiries.Application.Common.Templates;

public record NewsletterSubscribedEmailModel(string Email);

public class NewsletterSubscribedTemplate : IEmailTemplate<NewsletterSubscribedEmailModel>
{
    public string Subject(NewsletterSubscribedEmailModel model) => "You're subscribed!";

    public string Build(NewsletterSubscribedEmailModel model) => EmailLayout.Wrap($"""
        <p>Thanks for subscribing to our newsletter with {WebUtility.HtmlEncode(model.Email)}. You'll hear from us soon.</p>
        """);
}
