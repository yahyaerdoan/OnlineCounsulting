using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Tenancy.Domain;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.Abstractions;

public interface IModuleOfferingRepository : IAsyncRepository<ModuleOffering, Guid>
{
}
