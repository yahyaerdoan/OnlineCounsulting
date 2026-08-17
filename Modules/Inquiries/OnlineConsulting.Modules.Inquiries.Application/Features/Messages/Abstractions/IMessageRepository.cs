using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Inquiries.Domain;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Abstractions;

public interface IMessageRepository : IAsyncRepository<Message, Guid>
{
}
