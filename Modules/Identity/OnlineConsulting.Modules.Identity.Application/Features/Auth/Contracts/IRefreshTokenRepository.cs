using Core.PersistenceLayer.Repositories.IRepositories;
using RefreshTokenEntity = OnlineConsulting.Modules.Identity.Domain.RefreshToken;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.Contracts;

public interface IRefreshTokenRepository : IAsyncRepository<RefreshTokenEntity, Guid>
{
}
