using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.Services.Application;
using OnlineConsulting.Modules.Services.Domain;
using OnlineConsulting.Modules.Services.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Services.Infrastructure.Repositories;

public class ServiceMediaItemRepository(ServicesDbContext context) : EfRepositoryBase<ServiceMediaItem, Guid, ServicesDbContext>(context), IServiceMediaItemRepository
{
}
