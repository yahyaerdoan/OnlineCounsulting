using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlightsIntros.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain;
using OnlineConsulting.Modules.SiteContent.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Repositories;

public class FeatureHighlightsIntroRepository(SiteContentDbContext context) : EfRepositoryBase<FeatureHighlightsIntro, Guid, SiteContentDbContext>(context), IFeatureHighlightsIntroRepository
{
}
