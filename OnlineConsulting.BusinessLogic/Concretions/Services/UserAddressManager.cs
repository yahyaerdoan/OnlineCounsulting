using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ResultHandler.Implementations.Error;
using ResultHandler.Implementations.Success;
using OnlineConsulting.BusinessLogic.Abstractions.IServices;
using OnlineConsulting.BusinessLogic.Concretions.GenericServices;
using OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.UserAddressDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;
using ResultHandler.Core.Enums;

namespace OnlineConsulting.BusinessLogic.Concretions.Services;

public class UserAddressManager(IMapper mapper, IGenericRepository<UserAddress> repository, ISystemUserService systemUserService) : GenericService<UserAddress, IDto>(mapper, repository), IUserAddressService
{
    public async Task<IOperationResult> AddUserAddressAsync(CreateUserAddressDto dto)
    {
        var userResult = await systemUserService.GetCurrentUserAsync();
        if (!userResult.IsSuccessful || userResult.Data is null)
            return new ErrorResult("User not found or not logged in.", ResultStatus.Unauthorized);

        dto.UserId = userResult.Data.Id;

        if (dto.IsShippingAddress)
        {
            var any = await UserAnyShippingAddressAsync();
            if (any is not null)
            {
                var oldAddress = await _repository.GetByIdAsync(any);
                if (oldAddress is not null)
                {
                    oldAddress.IsShippingAddress = false;
                    await _repository.UpdateAsync(oldAddress);
                }
            }
        }
        if (dto.IsBillingAddress)
        {
            var any = await UserAnyBillingAddressAsync();
            if (any is not null)
            {
                var oldAddress = await _repository.GetByIdAsync(any);
                if (oldAddress is not null)
                {
                    oldAddress.IsBillingAddress = false;
                    await _repository.UpdateAsync(oldAddress);
                }
            }
        }

        var result = await AddAsync(dto);

        return result;
    }
    public async Task<IOperationResult> UpdateUserAddressAsync(UpdateUserAddressDto dto)
    {
        if (dto.IsShippingAddress)
        {
            var any = await UserAnyShippingAddressAsync();
            if (any is not null)
            {
                var oldAddress = await _repository.GetByIdAsync(any);
                if (oldAddress is not null)
                {
                    oldAddress.IsShippingAddress = false;
                    await _repository.UpdateAsync(oldAddress);
                }
            }
        }
        if (dto.IsBillingAddress)
        {
            var any = await UserAnyBillingAddressAsync();
            if (any is not null)
            {
                var oldAddress = await _repository.GetByIdAsync(any);
                if (oldAddress is not null)
                {
                    oldAddress.IsBillingAddress = false;
                    await _repository.UpdateAsync(oldAddress);
                }
            }
        }

        var result = await UpdateAsync(dto);
        return result;
    }
    public async Task<IOperationResult<IQueryable<ResultUserAddressDto>>> GetAddressesAsync(bool tracking = true, bool? status = true)
    {
        var userResult = await systemUserService.GetCurrentUserAsync();
        if (!userResult.IsSuccessful || userResult.Data is null)
            return new ErrorDataResult<IQueryable<ResultUserAddressDto>>("User not found or not logged in.", ResultStatus.Unauthorized);

        var userId = userResult.Data.Id;

        var result = await GetWhereAsync<ResultUserAddressDto>(x => x.UserId == userId, tracking, status);

        return result;
    }
    public async Task<IOperationResult<ResultUserAddressDto>> GetShippingAddressAsync(bool tracking = true, bool? status = true)
    {
        var userResult = await systemUserService.GetCurrentUserAsync();
        if (!userResult.IsSuccessful || userResult.Data is null)
            return new ErrorDataResult<ResultUserAddressDto>("User not found or not logged in.", ResultStatus.Unauthorized);

        var userId = userResult.Data.Id;

        var result = await GetFirstOrDefaultAsync<ResultUserAddressDto>(x => x.IsShippingAddress && x.UserId == userId, tracking);

        return result ?? new ErrorDataResult<ResultUserAddressDto>("Shipping address not found.", ResultStatus.NotFound);
    }
    public async Task<IOperationResult<ResultUserAddressDto>> GetBillingAddressAsync(bool tracking = true, bool? status = true)
    {
        var userResult = await systemUserService.GetCurrentUserAsync();
        if (!userResult.IsSuccessful || userResult.Data is null)
            return new ErrorDataResult<ResultUserAddressDto>("User not found or not logged in.", ResultStatus.Unauthorized);

        var userId = userResult.Data.Id;

        var result = await GetFirstOrDefaultAsync<ResultUserAddressDto>(x => x.IsBillingAddress && x.UserId == userId, tracking);

        return result ?? new ErrorDataResult<ResultUserAddressDto>("Billing address not found.", ResultStatus.NotFound);
    }
    public async Task<IOperationResult> SetBillingAddressAsync(SetBillingAddressDto dto)
    {
        var oldAddress = await _repository.GetByIdAsync(dto.OldAddressId);
        if (oldAddress is not null)
        {
            oldAddress.IsBillingAddress = false;
            await _repository.UpdateAsync(oldAddress);
        }
        var newAddress = await _repository.GetByIdAsync(dto.AddressId);
        if (newAddress is not null)
        {
            newAddress.IsBillingAddress = true;
            _ = await _repository.UpdateAsync(newAddress);
        }

        _ = await SaveAsync();
        return new SuccessResult("Billing address set successfully.");
    }
    public async Task<IOperationResult> SetShippingAddressAsync(SetShippingAddressDto dto)
    {
        var oldAddress = await _repository.GetByIdAsync(dto.OldAddressId);
        if (oldAddress is not null)
        {
            oldAddress.IsShippingAddress = false;
            await _repository.UpdateAsync(oldAddress);
        }
        var newAddress = await _repository.GetByIdAsync(dto.AddressId);
        if (newAddress is not null)
        {
            newAddress.IsShippingAddress = true;
            _ = await _repository.UpdateAsync(newAddress);
        }

        _ = await SaveAsync();
        return new SuccessResult("Shipping address set successfully.");
    }
    public async Task<string?> UserAnyShippingAddressAsync()
    {
        var result = await GetAddressesAsync();

        var addresses = result?.Data;
        if (addresses is null)
            return null;

        var id = await addresses.Where(x => x.IsShippingAddress).Select(x => (Guid?)x.Id).FirstOrDefaultAsync();
        return id?.ToString();
    }
    public async Task<string?> UserAnyBillingAddressAsync()
    {
        var result = await GetAddressesAsync();

        var addresses = result?.Data;
        if (addresses is null)
            return null;

        var id = await addresses.Where(x => x.IsBillingAddress).Select(x => (Guid?)x.Id).FirstOrDefaultAsync();
        return id?.ToString();
    }
}
