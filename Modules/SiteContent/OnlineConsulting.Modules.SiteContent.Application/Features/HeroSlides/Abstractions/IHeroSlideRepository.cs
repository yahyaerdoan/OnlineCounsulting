using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.SiteContent.Domain;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.HeroSlides.Abstractions;

public interface IHeroSlideRepository : IAsyncRepository<HeroSlide, Guid>
{
}
