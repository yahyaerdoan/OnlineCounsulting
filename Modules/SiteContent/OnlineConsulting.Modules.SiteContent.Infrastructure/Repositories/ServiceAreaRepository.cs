using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain;
using OnlineConsulting.Modules.SiteContent.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Repositories;

public class ServiceAreaRepository(SiteContentDbContext context) : EfRepositoryBase<ServiceArea, Guid, SiteContentDbContext>(context), IServiceAreaRepository
{
}
