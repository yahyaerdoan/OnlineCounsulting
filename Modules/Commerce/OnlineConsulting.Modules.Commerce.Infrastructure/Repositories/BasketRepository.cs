using Core.PersistenceLayer.Repositories.EfRepositories;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Abstractions;
using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.Modules.Commerce.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Commerce.Infrastructure.Repositories;

public class BasketRepository(CommerceDbContext context) : EfRepositoryBase<Basket, Guid, CommerceDbContext>(context), IBasketRepository
{
    public Task<Basket?> GetForOwnerAsync(Guid? userId, Guid? guestId, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        var query = Context.Baskets.Include(b => b.Items).AsQueryable();
        if (!enableTracking)
        {
            query = query.AsNoTracking();
        }

        return userId is { } uid
            ? query.FirstOrDefaultAsync(b => b.UserId == uid, cancellationToken)
            : query.FirstOrDefaultAsync(b => b.GuestId == guestId, cancellationToken);
    }
}
