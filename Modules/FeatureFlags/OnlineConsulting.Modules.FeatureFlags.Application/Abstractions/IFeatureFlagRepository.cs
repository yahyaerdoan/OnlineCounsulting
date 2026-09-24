using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.FeatureFlags.Domain;

namespace OnlineConsulting.Modules.FeatureFlags.Application.Abstractions;

/// <summary>Repository for the per-tenant flag override rows (see FeatureFlag for the override-vs-default semantics).</summary>
public interface IFeatureFlagRepository : IAsyncRepository<FeatureFlag, Guid>
{
}
