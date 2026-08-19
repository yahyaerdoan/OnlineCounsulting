namespace OnlineConsulting.SharedKernel.Authorization;

/// <summary>OnlineConsulting-specific operation claims not covered by Core.SecurityLayer's GeneralOperationClaims.</summary>
public static class GlobalOperationClaims
{
    public const string SuperAdmin = "Super Admin";

    /// <summary>Bare teammate role granted by default on invite acceptance (see CreateInviteCommand) - no
    /// elevated permission claims are granted to it, it relies on whatever default access an authenticated
    /// user with no special role already gets.</summary>
    public const string Member = "Member";
}
