namespace OnlineConsulting.SharedKernel.Authorization;

/// <summary>OnlineConsulting-specific operation claims not covered by Core.SecurityLayer's GeneralOperationClaims.</summary>
public static class GlobalOperationClaims
{
    public const string SuperAdmin = "Super Admin";
}
