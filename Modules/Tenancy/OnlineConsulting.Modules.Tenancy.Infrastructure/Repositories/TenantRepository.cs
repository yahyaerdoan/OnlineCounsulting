using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.Modules.Tenancy.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Repositories;

public class TenantRepository(TenancyDbContext context) : EfRepositoryBase<Tenant, Guid, TenancyDbContext>(context), ITenantRepository
{
}
