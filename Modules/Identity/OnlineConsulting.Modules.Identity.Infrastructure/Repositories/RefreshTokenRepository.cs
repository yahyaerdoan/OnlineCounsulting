using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.Contracts;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.Modules.Identity.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Identity.Infrastructure.Repositories;

public class RefreshTokenRepository(AppIdentityDbContext context)
    : EfRepositoryBase<RefreshToken, Guid, AppIdentityDbContext>(context), IRefreshTokenRepository
{
}
