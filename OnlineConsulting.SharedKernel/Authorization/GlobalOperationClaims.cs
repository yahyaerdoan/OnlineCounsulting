namespace OnlineConsulting.SharedKernel.Authorization;

/// <summary>OnlineConsulting-specific operation claims not covered by Core.SecurityLayer's GeneralOperationClaims.</summary>
public static class GlobalOperationClaims
{
    /// <summary>Cross-tenant platform authority; only SuperAdmin holds it, so Admin's per-tenant bypass can never reach Tenancy/Platform commands.</summary>
    public const string SuperAdmin = "Super Admin";

    /// <summary>Default teammate role on invite acceptance - no elevated claims granted.</summary>
    public const string Member = "Member";

    /// <summary>Storefront user - deliberately excluded from every /admin/* allowlist.</summary>
    public const string User = "User";
}
