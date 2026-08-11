using Microsoft.AspNetCore.Http;
using ResultHandler.Core.Abstractions;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.UserDtos;

namespace OnlineConsulting.BusinessLogic.Abstractions.IServices;

public interface ISystemUserService
{
    Task<IOperationResult<List<ResultUserDto>>> GetAllUsersAsync();
    Task<IOperationResult> CreateUserAsync(CreateUserDto createUserDto);
    Task<IOperationResult> LoginUserAsync(LoginUserDto loginUserDto);
    Task<IOperationResult> LoginAdminAsync(LoginUserDto loginUserDto);
    Task<IOperationResult<ResultUserDto>> GetCurrentUserAsync();
    Task<IOperationResult> LogoutUser();
    Task<IOperationResult<List<AssingARoleToUserDto>>> GetUserRolesAsync(string id);
    Task<IOperationResult> AssingARoleToUserAsync(string id, List<AssingARoleToUserDto> assingARoleToUserDtos);
    Task<IOperationResult> UpdateUserImageAsync(string id, IFormFile image);
    Task<IOperationResult> UpdateUserAsync(UpdateUserDto updateUserDto);
    Task<IOperationResult> DeleteSytemUserByIdAsync(string id);
    Task<IOperationResult> ChangePasswordAsync(ChangePasswordUserDto changePasswordUserDto);
    Task<IOperationResult<ResultUserDto>> ValidateApiCredentialsAsync(LoginUserDto loginUserDto);
}
