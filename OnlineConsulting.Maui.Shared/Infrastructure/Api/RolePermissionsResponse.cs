namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors GET /api/v1/roles/permissions - one entry per role.</summary>
public record RolePermissionsResponse(Guid RoleId, string RoleName, List<string> Permissions);
