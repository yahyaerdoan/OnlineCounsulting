using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.Modules.Tenancy.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Repositories;

public class ModuleOfferingRepository(TenancyDbContext context) : EfRepositoryBase<ModuleOffering, Guid, TenancyDbContext>(context), IModuleOfferingRepository
{
}
