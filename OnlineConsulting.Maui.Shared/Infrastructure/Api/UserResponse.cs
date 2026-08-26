namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors GET /api/users' response shape.</summary>
public record UserResponse(
    Guid Id, Guid TenantId, string UserName, string FirstName, string LastName, string Email,
    string? ImageUrl, bool IsActive, IReadOnlyList<string> Roles, IReadOnlyList<string> Permissions, bool IsSuperAdmin);
