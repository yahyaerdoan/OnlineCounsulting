using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Contracts;

public interface IUserAddressRepository : IAsyncRepository<UserAddress, Guid>
{
}
