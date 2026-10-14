using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.SiteContent.Application.Features.LandingPages.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain.Landing;
using OnlineConsulting.Modules.SiteContent.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Repositories.Landing;

public class LandingPageRepository(SiteContentDbContext context) : EfRepositoryBase<LandingPage, Guid, SiteContentDbContext>(context), ILandingPageRepository
{
}
