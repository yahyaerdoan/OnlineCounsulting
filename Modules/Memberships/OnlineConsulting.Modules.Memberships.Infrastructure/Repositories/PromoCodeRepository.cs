using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.Abstractions;
using OnlineConsulting.Modules.Memberships.Domain;
using OnlineConsulting.Modules.Memberships.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Memberships.Infrastructure.Repositories;

public class PromoCodeRepository(MembershipsDbContext context) : EfRepositoryBase<PromoCode, Guid, MembershipsDbContext>(context), IPromoCodeRepository
{
}
