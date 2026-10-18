using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.Modules.Tenancy.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Repositories;

public class TenantEmailDomainRepository(TenancyDbContext context) : EfRepositoryBase<TenantEmailDomain, Guid, TenancyDbContext>(context), ITenantEmailDomainRepository
{
}
