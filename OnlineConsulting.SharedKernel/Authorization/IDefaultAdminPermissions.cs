namespace OnlineConsulting.SharedKernel.Authorization;

/// <summary>A module's own default Admin permission grant, registered via DI so RoleSeeder can collect them without referencing every module's Application layer.</summary>
public interface IDefaultAdminPermissions
{
    string[] Permissions { get; }
}

public class DefaultAdminPermissions(string[] permissions) : IDefaultAdminPermissions
{
    public string[] Permissions { get; } = permissions;
}
