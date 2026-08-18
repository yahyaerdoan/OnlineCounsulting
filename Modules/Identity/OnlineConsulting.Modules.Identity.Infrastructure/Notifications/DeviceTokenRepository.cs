using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.Modules.Identity.Infrastructure.Persistence;
using OnlineConsulting.SharedKernel.Notifications;

namespace OnlineConsulting.Modules.Identity.Infrastructure.Notifications;

public class DeviceTokenRepository(AppIdentityDbContext context) : IDeviceTokenRepository
{
    public async Task RegisterAsync(Guid userId, string token, string platform, CancellationToken cancellationToken = default)
    {
        var existing = await context.DeviceTokens.FirstOrDefaultAsync(d => d.Token == token, cancellationToken);
        if (existing is not null)
        {
            existing.UserId = userId;
            existing.Platform = platform;
            existing.RegisteredAt = DateTimeOffset.UtcNow;
        }
        else
        {
            context.DeviceTokens.Add(new DeviceToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = token,
                Platform = platform,
                RegisteredAt = DateTimeOffset.UtcNow,
            });
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(string token, CancellationToken cancellationToken = default)
    {
        var existing = await context.DeviceTokens.FirstOrDefaultAsync(d => d.Token == token, cancellationToken);
        if (existing is null)
            return;

        context.DeviceTokens.Remove(existing);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<string>> GetTokensForUserAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await context.DeviceTokens.Where(d => d.UserId == userId).Select(d => d.Token).ToListAsync(cancellationToken);
}
