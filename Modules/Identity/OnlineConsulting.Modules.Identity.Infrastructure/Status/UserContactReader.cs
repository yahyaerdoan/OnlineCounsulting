using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Identity.Infrastructure.Persistence;
using OnlineConsulting.SharedKernel.Identity;

namespace OnlineConsulting.Modules.Identity.Infrastructure.Status;

/// <summary>Cross-module implementation of IUserContactReader, backing Commerce's order and Scheduling's appointment notifications.</summary>
public class UserContactReader(AppIdentityDbContext context) : IUserContactReader
{
    public Task<string?> GetEmailAsync(Guid userId, CancellationToken cancellationToken = default) =>
        context.Users.Where(u => u.Id == userId && u.DeletedDate == null).Select(u => u.Email).FirstOrDefaultAsync(cancellationToken);

    public Task<UserContact?> GetContactAsync(Guid userId, CancellationToken cancellationToken = default) =>
        context.Users.Where(u => u.Id == userId && u.DeletedDate == null)
            .Select(u => new UserContact(u.Id, u.Email, u.FirstName, u.LastName, u.UserName))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<UserContact>> GetContactsAsync(IReadOnlyCollection<Guid> userIds, CancellationToken cancellationToken = default) =>
        userIds.Count == 0
            ? []
            : await context.Users.Where(u => userIds.Contains(u.Id) && u.DeletedDate == null)
                .Select(u => new UserContact(u.Id, u.Email, u.FirstName, u.LastName, u.UserName))
                .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Guid>> FindUserIdsAsync(string term, int maxResults = 200, CancellationToken cancellationToken = default)
    {
        var trimmed = term.Trim();
        if (trimmed.Length == 0)
        {
            return [];
        }

        return await context.Users
            .Where(u => u.DeletedDate == null
                && (u.FirstName.Contains(trimmed) || u.LastName.Contains(trimmed) || (u.Email != null && u.Email.Contains(trimmed))
                    || (u.FirstName + " " + u.LastName).Contains(trimmed)))
            .Select(u => u.Id)
            .Take(maxResults)
            .ToListAsync(cancellationToken);
    }
}
