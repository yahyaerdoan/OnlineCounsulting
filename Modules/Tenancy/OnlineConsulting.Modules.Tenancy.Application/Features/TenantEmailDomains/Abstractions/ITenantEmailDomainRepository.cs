using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Tenancy.Domain;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.Abstractions;

public interface ITenantEmailDomainRepository : IAsyncRepository<TenantEmailDomain, Guid>
{
}
