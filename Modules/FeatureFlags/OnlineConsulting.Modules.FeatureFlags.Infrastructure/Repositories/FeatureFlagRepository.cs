using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.FeatureFlags.Application.Abstractions;
using OnlineConsulting.Modules.FeatureFlags.Domain;
using OnlineConsulting.Modules.FeatureFlags.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.FeatureFlags.Infrastructure.Repositories;

public class FeatureFlagRepository(FeatureFlagsDbContext context) : EfRepositoryBase<FeatureFlag, Guid, FeatureFlagsDbContext>(context), IFeatureFlagRepository
{
}
