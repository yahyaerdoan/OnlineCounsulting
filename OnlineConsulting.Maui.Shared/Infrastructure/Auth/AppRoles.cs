namespace OnlineConsulting.Maui.Shared.Infrastructure.Auth;

/// <summary>Role names as the API reports them - mirrors OnlineConsulting.SharedKernel.Authorization.GlobalOperationClaims,
/// duplicated here since this client project doesn't reference that server-side project.</summary>
public static class AppRoles
{
    /// <summary>Storefront user - the role every self-registered account gets.</summary>
    public const string User = "User";

    /// <summary>Cross-tenant platform authority - has a global bypass, ignores per-module permission toggles.</summary>
    public const string SuperAdmin = "Super Admin";
}
