namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors GET /api/v1/users/{id}/permission-overrides - same shape POSTed back to PUT.</summary>
public record UserPermissionOverridesResponse(List<string> RolePermissions, List<string> DeniedPermissions);
