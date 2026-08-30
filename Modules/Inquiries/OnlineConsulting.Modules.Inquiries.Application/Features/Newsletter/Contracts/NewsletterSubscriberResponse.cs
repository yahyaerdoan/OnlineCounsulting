using Hateoas;
using OnlineConsulting.Modules.Inquiries.Domain;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.Contracts;

/// <summary>Subscriber response as a class with required init properties, since records can't inherit the plain LinkedResponse class.</summary>
public class NewsletterSubscriberResponse : LinkedResponse
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
    public required DateTimeOffset CreatedDate { get; init; }

    public static NewsletterSubscriberResponse FromDomain(NewsletterSubscriber subscriber) => new()
    {
        Id = subscriber.Id,
        Email = subscriber.Email,
        CreatedDate = subscriber.CreatedDate,
    };
}
