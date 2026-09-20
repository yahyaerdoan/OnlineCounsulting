namespace OnlineConsulting.Modules.Identity.Application.Features.Users.Contracts;

/// <summary>RolePermissions is the full baseline the user's role(s) grant - DeniedPermissions is the subset
/// of that baseline this specific user has had revoked. Effective permissions = RolePermissions minus
/// DeniedPermissions (see RolePermissionResolver.ApplyUserOverridesAsync, applied at login/refresh).</summary>
public record UserPermissionOverridesResponse(List<string> RolePermissions, List<string> DeniedPermissions);
