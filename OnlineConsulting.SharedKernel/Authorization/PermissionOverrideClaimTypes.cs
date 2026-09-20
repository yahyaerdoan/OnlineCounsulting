namespace OnlineConsulting.SharedKernel.Authorization;

/// <summary>Per-user claim type for revoking one specific permission the user's role would otherwise
/// grant them (e.g. an Admin with "users.delete" denied). Stored on the user (AspNetUserClaims), not the
/// role - lets a tenant Owner narrow one team member without touching the shared Admin role definition.
/// Subtracted from the role-derived permission list at login/refresh (see RolePermissionResolver) -
/// AuthorizationAddingBehavior itself is unaware of this, it only ever sees the already-narrowed list.</summary>
public static class PermissionOverrideClaimTypes
{
    public const string Deny = "permission-deny";
}
