using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.Modules.Tenancy.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Repositories;

public class TenantSubscriptionItemRepository(TenancyDbContext context) : EfRepositoryBase<TenantSubscriptionItem, Guid, TenancyDbContext>(context), ITenantSubscriptionItemRepository
{
}
