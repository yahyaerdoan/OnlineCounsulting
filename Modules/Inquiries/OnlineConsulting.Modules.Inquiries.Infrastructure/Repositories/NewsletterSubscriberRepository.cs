using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.Contracts;
using OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.Abstractions;
using OnlineConsulting.Modules.Inquiries.Domain;
using OnlineConsulting.Modules.Inquiries.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Inquiries.Infrastructure.Repositories;

public class NewsletterSubscriberRepository(InquiriesDbContext context) : EfRepositoryBase<NewsletterSubscriber, Guid, InquiriesDbContext>(context), INewsletterSubscriberRepository
{
}
