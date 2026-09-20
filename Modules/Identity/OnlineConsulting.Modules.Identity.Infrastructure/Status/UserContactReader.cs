using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Identity.Infrastructure.Persistence;
using OnlineConsulting.SharedKernel.Identity;

namespace OnlineConsulting.Modules.Identity.Infrastructure.Status;

/// <summary>Cross-module implementation of IUserContactReader, backing Commerce's order-cancellation notifications.</summary>
public class UserContactReader(AppIdentityDbContext context) : IUserContactReader
{
    public Task<string?> GetEmailAsync(Guid userId, CancellationToken cancellationToken = default) =>
        context.Users.Where(u => u.Id == userId && u.DeletedDate == null).Select(u => u.Email).FirstOrDefaultAsync(cancellationToken);
}
