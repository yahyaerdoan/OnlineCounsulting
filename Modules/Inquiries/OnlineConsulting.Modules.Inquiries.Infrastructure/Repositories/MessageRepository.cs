using System.Linq.Expressions;
using Core.PersistenceLayer.Repositories.EfRepositories;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Abstractions;
using OnlineConsulting.Modules.Inquiries.Domain;
using OnlineConsulting.Modules.Inquiries.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Inquiries.Infrastructure.Repositories;

public class MessageRepository(InquiriesDbContext context) : EfRepositoryBase<Message, Guid, InquiriesDbContext>(context), IMessageRepository
{
    public Task<Message?> GetWithRepliesAsync(Expression<Func<Message, bool>> predicate, CancellationToken cancellationToken = default) =>
        Context.Messages
            .Include(m => m.Replies.OrderBy(r => r.CreatedDate))
            .AsNoTracking()
            .FirstOrDefaultAsync(predicate, cancellationToken);

    public Task<Message?> GetLatestGuestConversationAsync(string email, CancellationToken cancellationToken = default) =>
        Context.Messages
            .AsNoTracking()
            .Where(m => m.UserId == null && m.Email == email)
            .OrderByDescending(m => m.LastMessageAt)
            .FirstOrDefaultAsync(cancellationToken);

    /// <summary>Explicit Add: base UpdateAsync would treat the new reply (Id already set) as Modified.</summary>
    public async Task AddReplyAsync(Message message, MessageReply reply, CancellationToken cancellationToken = default)
    {
        _ = Context.MessageReplies.Add(reply);
        _ = Context.Messages.Update(message);
        _ = await Context.SaveChangesAsync(cancellationToken);
    }
}
