using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.SiteContent.Domain;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.Promotions.Abstractions;

public interface IPromotionRepository : IAsyncRepository<Promotion, Guid>
{
}
