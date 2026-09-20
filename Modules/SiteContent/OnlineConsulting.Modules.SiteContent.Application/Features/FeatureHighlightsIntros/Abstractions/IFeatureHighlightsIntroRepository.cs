using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.SiteContent.Domain;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlightsIntros.Abstractions;

public interface IFeatureHighlightsIntroRepository : IAsyncRepository<FeatureHighlightsIntro, Guid>
{
}
