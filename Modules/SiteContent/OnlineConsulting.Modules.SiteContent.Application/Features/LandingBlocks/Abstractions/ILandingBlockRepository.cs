using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.SiteContent.Domain.Landing;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.LandingBlocks.Abstractions;

public interface ILandingBlockRepository : IAsyncRepository<LandingBlock, Guid>
{
}
