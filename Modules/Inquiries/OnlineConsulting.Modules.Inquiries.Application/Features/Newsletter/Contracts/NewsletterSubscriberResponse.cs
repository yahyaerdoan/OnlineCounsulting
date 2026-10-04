using OnlineConsulting.Modules.Inquiries.Domain;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.Contracts;

public class NewsletterSubscriberResponse
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
