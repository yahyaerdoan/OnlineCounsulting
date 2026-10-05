namespace OnlineConsulting.SharedKernel.Authorization;

public class DefaultAdminPermissions(string[] permissions) : IDefaultAdminPermissions
{
    public string[] Permissions { get; } = permissions;
}
