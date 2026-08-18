using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Tenancy.Domain;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Abstractions;

public interface ITenantSubscriptionItemRepository : IAsyncRepository<TenantSubscriptionItem, Guid>
{
}
