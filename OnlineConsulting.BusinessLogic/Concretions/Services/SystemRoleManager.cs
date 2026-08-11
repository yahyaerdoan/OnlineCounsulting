using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ResultHandler.Implementations.Error;
using ResultHandler.Implementations.Success;
using OnlineConsulting.BusinessLogic.Abstractions.IServices;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.SystemRoleDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;
using ResultHandler.Core.Enums;

namespace OnlineConsulting.BusinessLogic.Concretions.Services;

public class SystemRoleManager(IMapper mapper, RoleManager<Role> roleManager) : ISystemRoleService
{
    public async Task<IOperationResult> CreateRoleAsync(CreateSystemRoleDto createSystemRoleDto)
    {
        if (createSystemRoleDto is null || createSystemRoleDto.GetType().GetProperties().All(p => p.GetValue(createSystemRoleDto) is null))
            return new ErrorResult($"The role could not be created. Please ensure the provided data is correct and try again.", ResultStatus.BadRequest);

        var role = mapper.Map<Role>(createSystemRoleDto);
        role.Id = Guid.NewGuid().ToString();

        var result = await roleManager.CreateAsync(role);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => $"{e.Code}: {e.Description}\n").ToList();
            var errorMessage = string.Join("\n", errors);
            return new ErrorResult($" {errorMessage} errors occurred while saving the role. Please try again later.", ResultStatus.BadRequest);
        }

        return new SuccessResult($"The role has been successfully created.", ResultStatus.Created);
    }

    public async Task<IOperationResult> DeleteRoleByIdAsync(string id)
    {
        var role = await roleManager.FindByIdAsync(id);

        if (role is null)
            return new ErrorDataResult<ResultSystemRoleDto>("No role data found.", ResultStatus.NotFound);
        var identityResult = await roleManager.DeleteAsync(role);

        if (!identityResult.Succeeded)
        {
            var errors = identityResult.Errors.Select(e => $"{e.Code}: {e.Description}\n").ToList();
            var errorMessage = string.Join("\n", errors);
            return new ErrorResult($" {errorMessage} errors occurred while deleting the role. Please try again later.", ResultStatus.InternalServerError);
        }

        return new SuccessResult($"The role has been successfully deleted.", ResultStatus.Ok);
    }

    public async Task<IOperationResult<List<ResultSystemRoleDto>>> GetAllRolesAsync()
    {
        var roles = await roleManager.Roles.ToListAsync();

        if (roles.Count == 0)
            return new ErrorDataResult<List<ResultSystemRoleDto>>("No role data found.", ResultStatus.NotFound);
        var result = mapper.Map<List<ResultSystemRoleDto>>(roles);

        return new SuccessDataResult<List<ResultSystemRoleDto>>(result, $"Role data retrieved successfully.", ResultStatus.Ok);
    }

    public async Task<IOperationResult<ResultSystemRoleDto>> GetRoleByIdAsync(string id)
    {
        var role = await roleManager.FindByIdAsync(id);

        if (role is null)
            return new ErrorDataResult<ResultSystemRoleDto>("No role data found.", ResultStatus.NotFound);

        var result = mapper.Map<ResultSystemRoleDto>(role);

        return new SuccessDataResult<ResultSystemRoleDto>(result, $"Role data retrieved successfully.", ResultStatus.Ok);
    }

    public async Task<IOperationResult> UpdateRoleAsync(UpdateSystemRoleDto updateSystemRoleDto)
    {

        if (updateSystemRoleDto is null || updateSystemRoleDto.GetType().GetProperties().All(p => p.GetValue(updateSystemRoleDto) is null))
            return new ErrorResult($"The role could not be created. Please ensure the provided data is correct and try again.", ResultStatus.BadRequest);


        var role = await roleManager.FindByIdAsync(updateSystemRoleDto.Id);
        if (role is null)
            return new ErrorResult($"The role could not be found. Please ensure the provided data is correct and try again.", ResultStatus.BadRequest);

        role.Name = updateSystemRoleDto.Name;
        role.Description = updateSystemRoleDto.Description;

        var result = await roleManager.UpdateAsync(role);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => $"{e.Code}: {e.Description}\n").ToList();
            var errorMessage = string.Join("\n", errors);
            return new ErrorResult($" {errorMessage} errors occurred while saving the role. Please try again later.", ResultStatus.BadRequest);
        }

        return new SuccessResult($"The role has been successfully created.", ResultStatus.Created);
    }
}
