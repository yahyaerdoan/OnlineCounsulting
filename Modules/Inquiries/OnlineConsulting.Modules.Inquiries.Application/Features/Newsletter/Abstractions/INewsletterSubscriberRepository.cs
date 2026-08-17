using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Inquiries.Domain;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.Abstractions;

public interface INewsletterSubscriberRepository : IAsyncRepository<NewsletterSubscriber, Guid>
{
}
