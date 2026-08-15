using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.SiteContent.Domain;

namespace OnlineConsulting.Modules.SiteContent.Application;

public interface IFeatureHighlightRepository : IAsyncRepository<FeatureHighlight, Guid>
{
}
