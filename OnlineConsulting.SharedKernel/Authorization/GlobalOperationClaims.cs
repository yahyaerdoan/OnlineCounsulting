namespace OnlineConsulting.SharedKernel.Authorization;

/// <summary>OnlineConsulting-specific operation claims not covered by Core.SecurityLayer's GeneralOperationClaims.</summary>
public static class GlobalOperationClaims
{
    public const string SuperAdmin = "Super Admin";

    /// <summary>The baseline role every registered user gets; self-service commands across modules gate on this rather than a per-module role.</summary>
    public const string User = "User";
}
