using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.FeatureFlags.Domain;

namespace OnlineConsulting.Modules.FeatureFlags.Application;

public interface IFeatureFlagRepository : IAsyncRepository<FeatureFlag, Guid>
{
}
