using OnlineConsulting.Modules.Inquiries.Domain;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.Contracts;

public record NewsletterSubscriberResponse(Guid Id, string Email, DateTimeOffset CreatedDate)
{
    public static NewsletterSubscriberResponse FromDomain(NewsletterSubscriber subscriber) => new(subscriber.Id, subscriber.Email, subscriber.CreatedDate);
}
