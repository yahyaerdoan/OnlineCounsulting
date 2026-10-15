using System.Linq.Expressions;
using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Inquiries.Domain;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Abstractions;

public interface IMessageRepository : IAsyncRepository<Message, Guid>
{
    /// <summary>Message with its replies, oldest first.</summary>
    Task<Message?> GetWithRepliesAsync(Expression<Func<Message, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Inserts the reply and updates the message in one save.</summary>
    Task AddReplyAsync(Message message, MessageReply reply, CancellationToken cancellationToken = default);
}
