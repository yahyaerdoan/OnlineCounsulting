namespace OnlineConsulting.SharedKernel.Authorization;

// Core.SecurityLayer's GeneralOperationClaims only defines "Admin" (an unconditional bypass in
// AuthorizationAddingBehavior). "Super Admin" is an OnlineConsulting-specific second tier, not part
// of that shared package, so it lives here instead of being a literal string repeated per command.
public static class GlobalOperationClaims
{
    public const string SuperAdmin = "Super Admin";
}
