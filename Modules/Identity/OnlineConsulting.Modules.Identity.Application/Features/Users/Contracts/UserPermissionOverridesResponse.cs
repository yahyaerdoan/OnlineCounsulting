namespace OnlineConsulting.Modules.Identity.Application.Features.Users.Contracts;

/// <summary>Effective permissions = RolePermissions minus DeniedPermissions (see RolePermissionResolver.ApplyUserOverridesAsync).</summary>
public record UserPermissionOverridesResponse(List<string> RolePermissions, List<string> DeniedPermissions);
