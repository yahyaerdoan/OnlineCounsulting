using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceOfferings.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain.Service;
using OnlineConsulting.Modules.SiteContent.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Repositories.Service;

public class ServiceOfferingRepository(SiteContentDbContext context) : EfRepositoryBase<ServiceOffering, Guid, SiteContentDbContext>(context), IServiceOfferingRepository
{
}
