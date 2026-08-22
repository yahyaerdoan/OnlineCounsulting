using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.SystemRole;

public class SystemRoleService(IApiClient apiClient) : ISystemRoleService
{
    private const string RolesPath = "/api/roles";

    public async Task<List<SystemRoleListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var roles = (await apiClient.GetAsync<List<RoleResponse>>(RolesPath, cancellationToken)).ResultData ?? [];
        return roles.Select(r => new SystemRoleListItemViewModel(r.Id, r.Name, r.Description)).ToList();
    }

    public async Task<UpdateSystemRoleViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<RoleResponse>($"{RolesPath}/{id}", cancellationToken);
        var role = result.ResultData;
        return role is null ? null : new UpdateSystemRoleViewModel { Id = role.Id, Name = role.Name, Description = role.Description };
    }

    public Task<ApiEnvelope> CreateAsync(CreateSystemRoleViewModel model, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync(RolesPath, new { model.Name, model.Description }, cancellationToken);

    public Task<ApiEnvelope> UpdateAsync(UpdateSystemRoleViewModel model, CancellationToken cancellationToken = default) =>
        apiClient.PutAsync($"{RolesPath}/{model.Id}", new { model.Name, model.Description }, cancellationToken);

    public Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiClient.DeleteAsync($"{RolesPath}/{id}", cancellationToken);

    public async Task<AssignRolePermissionsViewModel?> GetPermissionsAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var roleResult = await apiClient.GetAsync<RoleResponse>($"{RolesPath}/{roleId}", cancellationToken);
        var role = roleResult.ResultData;
        if (role is null)
        {
            return null;
        }

        var grantedPermissions = (await apiClient.GetAsync<List<string>>($"{RolesPath}/{roleId}/permissions", cancellationToken)).ResultData ?? [];
        var catalog = (await apiClient.GetAsync<Dictionary<string, string[]>>("/api/permissions", cancellationToken)).ResultData ?? [];

        var permissionsByModule = catalog.ToDictionary(
            module => module.Key,
            module => module.Value.Select(p => new PermissionCheckboxViewModel(p, grantedPermissions.Contains(p))).ToList());

        return new AssignRolePermissionsViewModel(role.Id, role.Name, permissionsByModule);
    }

    public Task<ApiEnvelope> AssignPermissionsAsync(Guid roleId, List<string> permissions, CancellationToken cancellationToken = default) =>
        apiClient.PutAsync($"{RolesPath}/{roleId}/permissions", new { Permissions = permissions }, cancellationToken);

    private record RoleResponse(Guid Id, string Name, string Description);
}
