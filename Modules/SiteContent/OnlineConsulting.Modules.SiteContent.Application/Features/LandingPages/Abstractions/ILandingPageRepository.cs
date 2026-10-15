using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.SiteContent.Domain.Landing;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.LandingPages.Abstractions;

public interface ILandingPageRepository : IAsyncRepository<LandingPage, Guid>
{
}
