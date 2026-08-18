using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Tenancy.Domain;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;

public interface ITenantSubscriptionRepository : IAsyncRepository<TenantSubscription, Guid>
{
}
