using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceProcessSteps.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain;
using OnlineConsulting.Modules.SiteContent.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Repositories;

public class ServiceProcessStepRepository(SiteContentDbContext context) : EfRepositoryBase<ServiceProcessStep, Guid, SiteContentDbContext>(context), IServiceProcessStepRepository
{
}
