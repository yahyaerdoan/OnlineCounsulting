using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.Modules.Tenancy.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Repositories;

public class TenantDomainRepository(TenancyDbContext context) : EfRepositoryBase<TenantDomain, Guid, TenancyDbContext>(context), ITenantDomainRepository
{
}
