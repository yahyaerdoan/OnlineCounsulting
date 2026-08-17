using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.SiteContent.Domain;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlights.Abstractions;

public interface IFeatureHighlightRepository : IAsyncRepository<FeatureHighlight, Guid>
{
}
