using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Abstractions;
using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.Modules.Commerce.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Commerce.Infrastructure.Repositories;

public class UserAddressRepository(CommerceDbContext context) : EfRepositoryBase<UserAddress, Guid, CommerceDbContext>(context), IUserAddressRepository
{
}
