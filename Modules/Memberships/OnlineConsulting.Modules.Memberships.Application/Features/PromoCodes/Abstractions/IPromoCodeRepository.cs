using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Memberships.Domain;

namespace OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.Abstractions;

public interface IPromoCodeRepository : IAsyncRepository<PromoCode, Guid>
{
}
