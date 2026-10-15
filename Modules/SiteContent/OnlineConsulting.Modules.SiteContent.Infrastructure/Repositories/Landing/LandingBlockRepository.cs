using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.SiteContent.Application.Features.LandingBlocks.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain.Landing;
using OnlineConsulting.Modules.SiteContent.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Repositories.Landing;

public class LandingBlockRepository(SiteContentDbContext context) : EfRepositoryBase<LandingBlock, Guid, SiteContentDbContext>(context), ILandingBlockRepository
{
}
