namespace OnlineConsulting.SharedKernel.Authorization;

// Core.SecurityLayer's GeneralOperationClaims only defines "Admin" (an unconditional bypass in
// AuthorizationAddingBehavior). "Super Admin" is an OnlineConsulting-specific second tier, not part
// of that shared package, so it lives here instead of being a literal string repeated per command.
public static class GlobalOperationClaims
{
    public const string SuperAdmin = "Super Admin";

    // The baseline role every registered user gets (see RegisterCommand). Self-service commands
    // across modules (e.g. Commerce's basket/address/order writes) gate on this rather than a
    // per-module role, since "any authenticated user acting on their own data" isn't a
    // module-specific concept.
    public const string User = "User";
}
