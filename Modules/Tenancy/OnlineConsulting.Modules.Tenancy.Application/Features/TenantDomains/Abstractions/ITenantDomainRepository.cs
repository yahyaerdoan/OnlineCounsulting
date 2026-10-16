using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Tenancy.Domain;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Abstractions;

public interface ITenantDomainRepository : IAsyncRepository<TenantDomain, Guid>
{
}
