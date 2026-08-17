using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.Services.Application.Contracts;
using OnlineConsulting.Modules.Services.Application.Abstractions;
using OnlineConsulting.Modules.Services.Domain;
using OnlineConsulting.Modules.Services.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Services.Infrastructure.Repositories;

public class ServiceRepository(ServicesDbContext context) : EfRepositoryBase<Service, Guid, ServicesDbContext>(context), IServiceRepository
{
}
