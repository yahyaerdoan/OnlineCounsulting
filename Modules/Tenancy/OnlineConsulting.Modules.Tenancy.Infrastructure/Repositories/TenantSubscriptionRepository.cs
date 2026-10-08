using System.Linq.Expressions;
using Core.PersistenceLayer.Repositories.EfRepositories;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.Modules.Tenancy.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Repositories;

public class TenantSubscriptionRepository(TenancyDbContext context) : EfRepositoryBase<TenantSubscription, Guid, TenancyDbContext>(context), ITenantSubscriptionRepository
{
    public Task<TenantSubscription?> GetWithItemsAsync(Expression<Func<TenantSubscription, bool>> predicate, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        var query = Context.TenantSubscriptions.Include(s => s.Items).AsQueryable();
        if (!enableTracking)
        {
            query = query.AsNoTracking();
        }

        return query.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyList<TenantSubscription>> GetAllWithItemsAsync(Expression<Func<TenantSubscription, bool>> predicate, CancellationToken cancellationToken = default) =>
        await Context.TenantSubscriptions.Include(s => s.Items).AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
}
