using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.Modules.Tenancy.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Repositories;

public class TenantSubscriptionRepository(TenancyDbContext context) : EfRepositoryBase<TenantSubscription, Guid, TenancyDbContext>(context), ITenantSubscriptionRepository
{
}
