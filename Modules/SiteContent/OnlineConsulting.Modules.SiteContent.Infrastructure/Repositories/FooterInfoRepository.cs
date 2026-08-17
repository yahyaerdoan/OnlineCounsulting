using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.SiteContent.Application.Features.FooterInfos.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain;
using OnlineConsulting.Modules.SiteContent.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Repositories;

public class FooterInfoRepository(SiteContentDbContext context) : EfRepositoryBase<FooterInfo, Guid, SiteContentDbContext>(context), IFooterInfoRepository
{
}
