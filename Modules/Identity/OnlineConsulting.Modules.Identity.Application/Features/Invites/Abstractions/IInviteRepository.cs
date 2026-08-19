using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Identity.Domain;

namespace OnlineConsulting.Modules.Identity.Application.Features.Invites.Abstractions;

public interface IInviteRepository : IAsyncRepository<Invite, Guid>
{
}
