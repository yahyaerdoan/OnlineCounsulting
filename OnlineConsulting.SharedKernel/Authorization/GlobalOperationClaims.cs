namespace OnlineConsulting.SharedKernel.Authorization;

/// <summary>OnlineConsulting-specific operation claims not covered by Core.SecurityLayer's GeneralOperationClaims.</summary>
public static class GlobalOperationClaims
{
    /// <summary>Cross-tenant platform authority (the app vendor) - distinct from PermissionClaimTypes.
    /// TenantFullAccess, Admin's per-tenant bypass. Only SuperAdmin holds this; Tenancy/Platform commands
    /// check it alone (AllowTenantBypass => false), so Admin's bypass can never reach them.</summary>
    public const string SuperAdmin = "Super Admin";

    /// <summary>Default teammate role on invite acceptance - no elevated claims granted.</summary>
    public const string Member = "Member";

    /// <summary>Storefront user - deliberately excluded from every /admin/* allowlist.</summary>
    public const string User = "User";
}
