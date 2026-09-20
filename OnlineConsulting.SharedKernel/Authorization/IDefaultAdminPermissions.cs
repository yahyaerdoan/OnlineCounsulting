namespace OnlineConsulting.SharedKernel.Authorization;

/// <summary>A module's own default Admin permission grant - registered by that module's DI setup so
/// RoleSeeder can grant them without referencing every module's Application layer. A module with no
/// registration (e.g. Tenancy, or Identity's own Roles claims) stays out of Admin's default grant.</summary>
public interface IDefaultAdminPermissions
{
    string[] Permissions { get; }
}

public class DefaultAdminPermissions(string[] permissions) : IDefaultAdminPermissions
{
    public string[] Permissions { get; } = permissions;
}
