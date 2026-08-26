namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors GET /api/users/{id}/roles - same shape POSTed back to PUT.</summary>
public record RoleAssignmentResponse(Guid RoleId, string RoleName, bool IsAssigned);
