namespace OnlineConsulting.UserInterface.Common;

public record UserSummaryViewModel
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
    public string Username { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;
    public List<RoleSummaryViewModel> Roles { get; set; } = [];
}

public record RoleSummaryViewModel(Guid Id, string Name, bool IsAssigned);

/// <summary>Mirrors the Api's GetCurrentUser response shape (GET /api/users/me) - kept UI-local rather than
/// referencing Identity.Application's UserResponse, since the UI talks to that endpoint over HTTP, not in-process.</summary>
public record CurrentUserResponse(Guid Id, Guid TenantId, string UserName, string FirstName, string LastName, string Email, string? ImageUrl, IReadOnlyList<string> Roles, IReadOnlyList<string> Permissions);

/// <summary>Mirrors the Api's GetUserRoles response shape (GET /api/users/{id}/roles).</summary>
public record RoleAssignmentResponse(Guid RoleId, string RoleName, bool IsAssigned);

public static class UserSummaryExtensions
{
    public static UserSummaryViewModel ToUserSummaryViewModel(this CurrentUserResponse summary) => new()
    {
        Id = summary.Id,
        TenantId = summary.TenantId,
        Username = summary.UserName,
        FirstName = summary.FirstName,
        LastName = summary.LastName,
        Email = summary.Email,
        ImageUrl = summary.ImageUrl ?? string.Empty,
    };
}
