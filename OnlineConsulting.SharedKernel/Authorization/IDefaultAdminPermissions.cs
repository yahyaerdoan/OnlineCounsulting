namespace OnlineConsulting.SharedKernel.Authorization;

/// <summary>A module's own default Admin permission grant, registered via DI so RoleBootstrapper can collect them without referencing every module's Application layer.</summary>
public interface IDefaultAdminPermissions
{
    string[] Permissions { get; }
}
