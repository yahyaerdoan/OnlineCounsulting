using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Inquiries.Domain;

namespace OnlineConsulting.Modules.Inquiries.Application;

public interface IMessageRepository : IAsyncRepository<Message, Guid>
{
}
