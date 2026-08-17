using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.SiteContent.Domain;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FooterInfos.Abstractions;

public interface IFooterInfoRepository : IAsyncRepository<FooterInfo, Guid>
{
}
