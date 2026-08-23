namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors GET /api/users/me's response shape.</summary>
public record CurrentUserResponse(Guid Id, Guid TenantId, string UserName, string FirstName, string LastName, string Email, string? ImageUrl, IReadOnlyList<string> Roles, IReadOnlyList<string> Permissions, bool IsSuperAdmin);
