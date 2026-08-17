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
        if (role is null)
            return null;

        return new UpdateSystemRoleViewModel { Id = role.Id, Name = role.Name, Description = role.Description };
    }

    public Task<ApiEnvelope> CreateAsync(CreateSystemRoleViewModel model, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync(RolesPath, new { model.Name, model.Description }, cancellationToken);

    public Task<ApiEnvelope> UpdateAsync(UpdateSystemRoleViewModel model, CancellationToken cancellationToken = default) =>
        apiClient.PutAsync($"{RolesPath}/{model.Id}", new { model.Name, model.Description }, cancellationToken);

    public Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiClient.DeleteAsync($"{RolesPath}/{id}", cancellationToken);

    private record RoleResponse(Guid Id, string Name, string Description);
}
