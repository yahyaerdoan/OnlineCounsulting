using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.Identity.Application.Features.Invites.Abstractions;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.Modules.Identity.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Identity.Infrastructure.Repositories;

public class InviteRepository(AppIdentityDbContext context) : EfRepositoryBase<Invite, Guid, AppIdentityDbContext>(context), IInviteRepository
{
}
