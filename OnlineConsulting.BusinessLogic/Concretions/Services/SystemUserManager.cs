using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ResultHandler.Core.Abstractions;
using ResultHandler.Core.Enums;
using ResultHandler.Implementations.Error;
using ResultHandler.Implementations.Success;
using OnlineConsulting.BusinessLogic.Abstractions.IServices;
using OnlineConsulting.BusinessLogic.Abstractions.IStorageServices.IStorages;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.SystemRoleDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.UserDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using System.Security.Claims;
namespace OnlineConsulting.BusinessLogic.Concretions.Services;

public class SystemUserManager(IMapper mapper, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<Role> roleManager, IStorageService storageService) : ISystemUserService
{
    private const string DefaultRoleName = "User";

    public async Task<IOperationResult<List<ResultUserDto>>> GetAllUsersAsync()
    {
        var users = await userManager.Users.ToListAsync();

        if (users.Count == 0)
            return new ErrorDataResult<List<ResultUserDto>>("No user data found.", ResultStatus.NotFound);
        var resultUserList = new List<ResultUserDto>();
        foreach (var user in users)
        {
            var rolesResult = await GetUserRolesByIdAsync(user.Id);
            var userDto = new ResultUserDto()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Roles = rolesResult.Data ?? []
            };
            resultUserList.Add(userDto);
        }

        return new SuccessDataResult<List<ResultUserDto>>(resultUserList, $"User data retrieved successfully.", ResultStatus.Ok);
    }
    public async Task<IOperationResult> CreateUserAsync(CreateUserDto createUserDto)
    {
        var user = PrepareUserEntity(createUserDto);

        var result = await userManager.CreateAsync(user, createUserDto.Password);
        if (!result.Succeeded)
            return MapIdentityErrors(result);

        if (!await roleManager.RoleExistsAsync(DefaultRoleName))
            await roleManager.CreateAsync(new Role { Name = DefaultRoleName });

        var roleResult = await userManager.AddToRoleAsync(user, DefaultRoleName);
        if (!roleResult.Succeeded)
        {
            var errors = string.Join("; ", roleResult.Errors.Select(e => e.Description));
            return new ErrorResult($"User created, but role assignment failed: {errors}", ResultStatus.InternalServerError);
        }

        return new SuccessResult("The user has been successfully created.", ResultStatus.Created);
    }
    public async Task<IOperationResult> LoginUserAsync(LoginUserDto loginUserDto)
    {
        if (string.IsNullOrWhiteSpace(loginUserDto.UserNameOrEmail) || string.IsNullOrWhiteSpace(loginUserDto.Password))
            return BadRequest("Login information is missing. Please try again.");

        var user = await FindUserAsync(loginUserDto.UserNameOrEmail);
        if (user is null)
            return BadRequest("Invalid credentials. Please check your username and password.");

        var signInResult = await ValidateUserCredentialsAsync(user, loginUserDto.Password, loginUserDto.RememberMe);

        if (signInResult.IsLockedOut)
            return await GenerateLockoutMessageAsync(user);

        if (!signInResult.Succeeded)
            return BadRequest("Invalid credentials. Please check your username and password.");

        return new SuccessResult($"Welcome back, {user.FirstName} {user.LastName}!", ResultStatus.Ok);
    }
    public async Task<IOperationResult<ResultUserDto>> GetCurrentUserAsync()
    {
        var identity = httpContextAccessor.HttpContext?.User?.Identity;
        if (identity is not { IsAuthenticated: true })
            return new ErrorDataResult<ResultUserDto>($"User not found. Please log in and try again.", ResultStatus.Unauthorized);

        var username = identity.Name
            ?? httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
        if (string.IsNullOrEmpty(username))
            return new ErrorDataResult<ResultUserDto>("User not found.", ResultStatus.NotFound);

        var user = await FindUserAsync(username);

        if (user is null)
            return new ErrorDataResult<ResultUserDto>($"The user could not be find. Please ensure the provided data is correct and try again.", ResultStatus.BadRequest);

        var resultUser = mapper.Map<ResultUserDto>(user);

        return new SuccessDataResult<ResultUserDto>(resultUser, "User found.", ResultStatus.Ok);
    }
    public async Task<IOperationResult> LogoutUser()
    {
        await signInManager.SignOutAsync();

        return new SuccessResult("You've been logged out. See you again soon!", ResultStatus.Ok);
    }
    public async Task<IOperationResult<List<AssingARoleToUserDto>>> GetUserRolesAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
            return new ErrorDataResult<List<AssingARoleToUserDto>>("User not found.", ResultStatus.NotFound);

        var roles = await roleManager.Roles.ToListAsync();
        if (roles.Count == 0)
            return new ErrorDataResult<List<AssingARoleToUserDto>>("Roles not found.", ResultStatus.NotFound);

        var userRoles = await userManager.GetRolesAsync(user);

        var roleList = roles.Select(role => new AssingARoleToUserDto
        {
            Id = role.Id,
            RoleName = role.Name ?? string.Empty,
            Exist = userRoles.Contains(role.Name ?? string.Empty)

        }).ToList();
        return new SuccessDataResult<List<AssingARoleToUserDto>>(roleList, "User roles retrived succesfully.", ResultStatus.Ok);
    }
    public async Task<IOperationResult> AssingARoleToUserAsync(string id, List<AssingARoleToUserDto> assingARoleToUserDtos)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
            return new ErrorResult($"The user could not be find. Please ensure the provided data is correct and try again.", ResultStatus.BadRequest);

        foreach (var role in assingARoleToUserDtos)
        {
            if (role.Exist)
            {
                await userManager.AddToRoleAsync(user, role.RoleName);
            }
            else
            {
                await userManager.RemoveFromRoleAsync(user, role.RoleName);
            }
        }
        return new SuccessResult("New permissions added successfully.", ResultStatus.Ok);
    }
    public async Task<IOperationResult> UpdateUserAsync(UpdateUserDto updateUserDto)
    {
        var user = await userManager.FindByIdAsync(updateUserDto.Id);

        if (user is null)
            return new ErrorResult($"Failed to map the provided user data. Please ensure the input is valid and try again.", ResultStatus.BadRequest);

        user.FirstName = updateUserDto.FirstName;
        user.LastName = updateUserDto.LastName;
        user.ImageUrl = updateUserDto.ImageUrl;
        user.Status = updateUserDto.Status;
        user.UpdatedDate = DateTime.Now;
        var currentUser = await GetCurrentUserAsync();
        user.UpdatedBy = currentUser.Data?.Username;

        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
            return new ErrorResult($"An error occurred while updating the user. Please try again later.", ResultStatus.InternalServerError);

        return new SuccessResult($"The user has been successfully updated.", ResultStatus.Ok);
    }
    public async Task<IOperationResult> UpdateUserImageAsync(string id, IFormFile image)
    {
        var user = await userManager.FindByIdAsync(id);


        if (user is null)
        {
            return new ErrorResult("The requested 'user' information could not be found. Please try again.", ResultStatus.NotFound);
        }

        // Validate image before processing
        if (image is null || image.Length == 0)
        {
            return new ErrorResult("No image was provided. Please select a valid image file to update the photo.", ResultStatus.BadRequest);
        }

        var allowedMimeTypes = new HashSet<string> { "image/jpeg", "image/jpg", "image/png", "image/gif" };

        // Check MIME type validity
        if (!allowedMimeTypes.Contains(image.ContentType))
        {
            return new ErrorResult("Invalid image format. Please upload an image in JPEG, JPG, PNG, or GIF format.", ResultStatus.BadRequest);
        }

        // Delete the existing image only if a valid new image is provided
        if (user.ImageUrl != "/Resource/LocalStorage/DefaultImages/defaultUserImage.png")
        {
            if (!string.IsNullOrEmpty(user.ImageUrl))
            {
                var rootPath = Directory.GetCurrentDirectory();
                var filePath = Path.Combine(rootPath, "wwwroot", user.ImageUrl.TrimStart('/'));

                if (File.Exists(filePath))
                {
                    await storageService.DeleteAsync(user.ImageUrl);
                }
            }
        }
        // Upload the new image
        var uploadedImageRoute = await storageService.UploadAsync("Resource/LocalStorage/User-Images", image);

        if (!string.IsNullOrEmpty(uploadedImageRoute.Data.FullPath))
        {
            user.ImageUrl = $"/{uploadedImageRoute.Data.TargetFolderPathOrContainerName}/{uploadedImageRoute.Data.FileName}";
        }
        else
        {
            return new ErrorResult("The image could not be uploaded. Please try again with a valid file.", ResultStatus.BadRequest);
        }

        var resultUser = await userManager.UpdateAsync(user);

        return resultUser.Succeeded
            ? new SuccessResult("The user profile image has been successfully updated.")
            : new ErrorResult("An unexpected error occurred while updating the user profile image. Please try again later.", ResultStatus.InternalServerError);
    }
    public async Task<IOperationResult> DeleteSytemUserByIdAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id);

        if (user is null)
            return new ErrorDataResult<ResultUserDto>("No user data found.", ResultStatus.NotFound);

        var identityResult = await userManager.DeleteAsync(user);

        if (!identityResult.Succeeded)
        {
            var errors = identityResult.Errors.Select(e => $"{e.Code}: {e.Description}\n").ToList();
            var errorMessage = string.Join("\n", errors);
            return new ErrorResult($" {errorMessage} errors occurred while deleting the role. Please try again later.", ResultStatus.InternalServerError);
        }

        return new SuccessResult($"The user has been successfully deleted.", ResultStatus.Ok);
    }
    public async Task<IOperationResult> LoginAdminAsync(LoginUserDto loginUserDto)
    {
        if (loginUserDto is null || loginUserDto.GetType().GetProperties().All(p => p.GetValue(loginUserDto) is null))
            return new ErrorResult($"The user could not be find. Please ensure the provided data is correct and try again.", ResultStatus.BadRequest);

        var user = await FindUserAsync(loginUserDto.UserNameOrEmail);

        if (user is null)
            return new ErrorResult($"The user could not be find. Please ensure the provided data is correct and try again.", ResultStatus.BadRequest);

        var userRolesResult = await GetUserRolesAsync(user.Id);

        var isAdminOrSuperAdmin = userRolesResult.Data?.Any(role => (role.RoleName == "Admin" || role.RoleName == "SuperAdmin") && role.Exist) ?? false;
        if (isAdminOrSuperAdmin)
        {
            var signInResult = await ValidateUserCredentialsAsync(user, loginUserDto.Password, loginUserDto.RememberMe);
            if (signInResult.IsLockedOut)
                return await GenerateLockoutMessageAsync(user);

            if (!signInResult.Succeeded)
                return new ErrorResult($"The user could not be find. Please ensure the provided data is correct and try again.", ResultStatus.BadRequest);

            return new SuccessResult($"Welcome back, {user.FirstName} {user.LastName}!", ResultStatus.Ok);
        }
        return new ErrorResult($"The user could not be find. Please ensure the provided data is correct and try again.", ResultStatus.BadRequest);
    }
    public async Task<IOperationResult<ResultUserDto>> ValidateApiCredentialsAsync(LoginUserDto loginUserDto)
    {
        if (string.IsNullOrWhiteSpace(loginUserDto.UserNameOrEmail) || string.IsNullOrWhiteSpace(loginUserDto.Password))
            return new ErrorDataResult<ResultUserDto>("Login information is missing. Please try again.", ResultStatus.BadRequest);

        var user = await FindUserAsync(loginUserDto.UserNameOrEmail);
        if (user is null)
            return new ErrorDataResult<ResultUserDto>("Invalid credentials. Please check your username and password.", ResultStatus.BadRequest);

        var signInResult = await signInManager.CheckPasswordSignInAsync(user, loginUserDto.Password, lockoutOnFailure: true);

        if (signInResult.IsLockedOut)
        {
            var lockoutResult = await GenerateLockoutMessageAsync(user);
            return new ErrorDataResult<ResultUserDto>(lockoutResult.Title, ResultStatus.Forbidden);
        }

        if (!signInResult.Succeeded)
            return new ErrorDataResult<ResultUserDto>("Invalid credentials. Please check your username and password.", ResultStatus.BadRequest);

        var rolesResult = await GetUserRolesByIdAsync(user.Id);

        var resultUser = mapper.Map<ResultUserDto>(user);
        resultUser.Roles = rolesResult.Data ?? [];

        return new SuccessDataResult<ResultUserDto>(resultUser, "Credentials validated successfully.", ResultStatus.Ok);
    }
    public async Task<IOperationResult> ChangePasswordAsync(ChangePasswordUserDto changePasswordUserDto)
    {
        var user = await userManager.FindByIdAsync(changePasswordUserDto.Id);
        if (user is null)
            return new ErrorResult("User not found.", ResultStatus.NotFound);

        //user.UpdatedDate = DateTime.Now;
        //var currentUser = await GetCurrentUserAsync();
        //user.UpdatedBy = currentUser.Data.Username;

        var result = await userManager.ChangePasswordAsync(user, changePasswordUserDto.CurrentPassword, changePasswordUserDto.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return new ErrorResult($"Failed to change password: {errors}", ResultStatus.BadRequest);
        }

        return new SuccessResult("Password changed successfully.", ResultStatus.Ok);
    }

    #region PrivateHelperMethods
    private async Task<SignInResult> ValidateUserCredentialsAsync(User user, string password, bool rememberMe) => await signInManager.PasswordSignInAsync(user, password, rememberMe, true);
    private async Task<User?> FindUserAsync(string usernameOrEmail)
    {
        return await userManager.FindByNameAsync(usernameOrEmail)
            ?? await userManager.Users.Where(u => u.Email == usernameOrEmail).FirstOrDefaultAsync();
    }
    private async Task<IOperationResult<List<ResultSystemRoleDto>>> GetUserRolesByIdAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
            return new ErrorDataResult<List<ResultSystemRoleDto>>($"The user could not be find. " +
                $"Please ensure the provided data is correct and try again.", ResultStatus.BadRequest);

        var userRoleNames = await userManager.GetRolesAsync(user);

        var userRoles = await roleManager.Roles
            .Where(role => userRoleNames.Contains(role.Name ?? string.Empty))
            .ToListAsync();

        var result = mapper.Map<List<ResultSystemRoleDto>>(userRoles);
        return new SuccessDataResult<List<ResultSystemRoleDto>>(result, "User roles retrived successfully.", ResultStatus.Ok);
    }
    private async Task<IOperationResult> GenerateLockoutMessageAsync(User user)
    {
        var lockoutEnd = await userManager.GetLockoutEndDateAsync(user);
        var remaining = lockoutEnd?.UtcDateTime - DateTime.UtcNow;

        var message = remaining.HasValue && remaining.Value.TotalSeconds > 0
            ? $"Your account is locked. Try again in {remaining.Value.Minutes} minute(s)."
            : "Your account is currently locked. Please try again later.";

        return new ErrorResult(message, ResultStatus.Forbidden);
    }
    private static ErrorResult BadRequest(string message) => new(message, ResultStatus.BadRequest);
    private User PrepareUserEntity(CreateUserDto dto)
    {
        var user = mapper.Map<User>(dto);
        user.Id = Guid.NewGuid().ToString();
        user.ImageUrl = "/Resource/LocalStorage/DefaultImages/defaultUserImage.png";
        return user;
    }
    private static IOperationResult MapIdentityErrors(IdentityResult result)
    {
        var validationErrors = result.Errors.Select(e => new ValidationRule
        {
            Description = e.Description,
            IsValid = false
        }).ToList();

        return new ErrorDataResult<List<ValidationRule>>(validationErrors,
            "We couldn't create your account due to the following issues. Please review and try again.",
            ResultStatus.BadRequest);
    }
    #endregion
}

public class ValidationRule
{
    public string Description { get; set; } = string.Empty;
    public bool IsValid { get; set; }
    public string? FieldName { get; set; }
}
