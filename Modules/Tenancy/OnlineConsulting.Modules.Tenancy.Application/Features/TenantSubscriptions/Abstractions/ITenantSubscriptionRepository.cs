using System.Linq.Expressions;
using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Tenancy.Domain;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;

public interface ITenantSubscriptionRepository : IAsyncRepository<TenantSubscription, Guid>
{
    /// <summary>The first matching subscription loaded with its modules, for showing it or changing them.</summary>
    Task<TenantSubscription?> GetWithItemsAsync(Expression<Func<TenantSubscription, bool>> predicate, bool enableTracking = true, CancellationToken cancellationToken = default);

    /// <summary>Every matching subscription loaded with its modules, read-only.</summary>
    Task<IReadOnlyList<TenantSubscription>> GetAllWithItemsAsync(Expression<Func<TenantSubscription, bool>> predicate, CancellationToken cancellationToken = default);
}
