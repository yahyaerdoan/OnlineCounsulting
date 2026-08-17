using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceOfferings.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceOfferings.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain;
using OnlineConsulting.Modules.SiteContent.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Repositories;

public class ServiceOfferingRepository(SiteContentDbContext context) : EfRepositoryBase<ServiceOffering, Guid, SiteContentDbContext>(context), IServiceOfferingRepository
{
}
