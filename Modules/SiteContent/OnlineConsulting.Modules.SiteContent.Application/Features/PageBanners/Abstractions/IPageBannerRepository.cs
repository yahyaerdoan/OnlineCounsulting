using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.SiteContent.Domain;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.PageBanners.Abstractions;

public interface IPageBannerRepository : IAsyncRepository<PageBanner, Guid>
{
}
