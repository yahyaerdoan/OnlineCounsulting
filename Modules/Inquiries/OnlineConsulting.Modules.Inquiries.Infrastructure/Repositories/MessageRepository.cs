using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Contracts;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Abstractions;
using OnlineConsulting.Modules.Inquiries.Domain;
using OnlineConsulting.Modules.Inquiries.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Inquiries.Infrastructure.Repositories;

public class MessageRepository(InquiriesDbContext context) : EfRepositoryBase<Message, Guid, InquiriesDbContext>(context), IMessageRepository
{
}
