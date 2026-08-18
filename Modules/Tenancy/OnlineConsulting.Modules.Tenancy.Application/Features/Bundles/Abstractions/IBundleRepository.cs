using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Tenancy.Domain;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Bundles.Abstractions;

public interface IBundleRepository : IAsyncRepository<Bundle, Guid>
{
}
