using OnlineConsulting.DataTransferObject.Concretions.Dtos.SystemRoleDtos;
using ResultHandler.Core.Abstractions;

namespace OnlineConsulting.BusinessLogic.Abstractions.IServices;

public interface ISystemRoleService
{
    Task<IOperationResult> CreateRoleAsync(CreateSystemRoleDto createSystemRoleDto);
    Task<IOperationResult<List<ResultSystemRoleDto>>> GetAllRolesAsync();
    Task<IOperationResult<ResultSystemRoleDto>> GetRoleByIdAsync(string id);
    Task<IOperationResult> UpdateRoleAsync(UpdateSystemRoleDto updateSystemRoleDto);
    Task<IOperationResult> DeleteRoleByIdAsync(string id);
}
