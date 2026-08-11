using OnlineConsulting.BusinessLogic.Abstractions.IGenericServices;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.UserAddressDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;
namespace OnlineConsulting.BusinessLogic.Abstractions.IServices;

public interface IUserAddressService : IGenericService<UserAddress, IDto>
{
    Task<string?> UserAnyBillingAddressAsync();
    Task<string?> UserAnyShippingAddressAsync();
    Task<IOperationResult> AddUserAddressAsync(CreateUserAddressDto dto);
    Task<IOperationResult> UpdateUserAddressAsync(UpdateUserAddressDto dto);
    Task<IOperationResult> SetBillingAddressAsync(SetBillingAddressDto dto);
    Task<IOperationResult> SetShippingAddressAsync(SetShippingAddressDto dto);
    Task<IOperationResult<IQueryable<ResultUserAddressDto>>> GetAddressesAsync(bool tracking = true, bool? status = true);
    Task<IOperationResult<ResultUserAddressDto>> GetShippingAddressAsync(bool tracking = true, bool? status = true);
    Task<IOperationResult<ResultUserAddressDto>> GetBillingAddressAsync(bool tracking = true, bool? status = true);
}
