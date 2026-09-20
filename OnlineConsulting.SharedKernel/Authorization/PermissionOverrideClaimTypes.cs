namespace OnlineConsulting.SharedKernel.Authorization;

/// <summary>Per-user claim that revokes one role-granted permission (stored on AspNetUserClaims); subtracted from the role list at login/refresh, so AuthorizationAddingBehavior only ever sees the already-narrowed result.</summary>
public static class PermissionOverrideClaimTypes
{
    public const string Deny = "permission-deny";
}
