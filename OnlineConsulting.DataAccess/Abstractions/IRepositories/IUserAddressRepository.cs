using OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataAccess.Abstractions.IRepositories;

public interface IUserAddressRepository : IGenericRepository<UserAddress>
{
    //public Task<UserAddress> GetUserAddressByIdAsync(string id);
    //public Task<List<UserAddress>> GetUserAddressesByUserIdAsync(string userId);
    //public Task<bool> AddUserAddressAsync(UserAddress userAddress);
    //public Task<bool> UpdateUserAddressAsync(UserAddress userAddress);
    //public Task<bool> DeleteUserAddressAsync(string id);
}
