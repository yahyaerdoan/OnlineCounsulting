using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.SystemUser;

public class SystemUserService(IApiClient apiClient) : ISystemUserService
{
    private const string UsersPath = "/api/users";
    private const string RolesPath = "/api/roles";

    public async Task<List<SystemUserListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var usersTask = apiClient.GetAsync<List<UserResponse>>(UsersPath, cancellationToken);
        var rolesTask = apiClient.GetAsync<List<RoleResponse>>(RolesPath, cancellationToken);
        await Task.WhenAll(usersTask, rolesTask);

        var rolesByName = (rolesTask.Result.ResultData ?? []).ToDictionary(r => r.Name);
        var users = usersTask.Result.ResultData ?? [];

        return users.Select(u => new SystemUserListItemViewModel(
            u.Id,
            u.UserName,
            u.FirstName,
            u.LastName,
            u.Email,
            u.Roles.Select(roleName => rolesByName.TryGetValue(roleName, out var role)
                ? new SystemUserRoleViewModel(role.Name, role.Description)
                : new SystemUserRoleViewModel(roleName, string.Empty)).ToList()))
            .ToList();
    }

    public async Task<List<RoleAssignmentViewModel>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<List<RoleAssignmentResponse>>($"{UsersPath}/{userId}/roles", cancellationToken);
        var assignments = result.ResultData ?? [];
        return assignments.Select(a => new RoleAssignmentViewModel { RoleId = a.RoleId, RoleName = a.RoleName, IsAssigned = a.IsAssigned }).ToList();
    }

    public Task<ApiEnvelope> AssignRolesAsync(Guid userId, List<RoleAssignmentViewModel> assignments, CancellationToken cancellationToken = default) =>
        apiClient.PutAsync($"{UsersPath}/{userId}/roles", new
        {
            RoleAssignments = assignments.Select(a => new { a.RoleId, a.RoleName, a.IsAssigned }),
        }, cancellationToken);

    public Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiClient.DeleteAsync($"{UsersPath}/{id}", cancellationToken);

    private record UserResponse(Guid Id, Guid TenantId, string UserName, string FirstName, string LastName, string Email, string? ImageUrl, List<string> Roles);
    private record RoleResponse(Guid Id, string Name, string Description);
    private record RoleAssignmentResponse(Guid RoleId, string RoleName, bool IsAssigned);
}
