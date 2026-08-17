using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Services.Domain;

namespace OnlineConsulting.Modules.Services.Application.Abstractions;

public interface IServiceRepository : IAsyncRepository<Service, Guid>
{
}
