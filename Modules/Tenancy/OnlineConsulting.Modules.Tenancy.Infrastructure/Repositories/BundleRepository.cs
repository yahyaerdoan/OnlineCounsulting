using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.Tenancy.Application.Features.Bundles.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.Modules.Tenancy.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Repositories;

public class BundleRepository(TenancyDbContext context) : EfRepositoryBase<Bundle, Guid, TenancyDbContext>(context), IBundleRepository
{
}
