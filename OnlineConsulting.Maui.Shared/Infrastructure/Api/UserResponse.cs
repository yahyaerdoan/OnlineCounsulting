using OnlineConsulting.Maui.Shared.Infrastructure.Hateoas;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors GET /api/v1/users' response shape.</summary>
public record UserResponse(Guid Id, Guid TenantId, string UserName, string FirstName, string LastName, string Email, string? ImageUrl, bool IsActive,
    IReadOnlyList<string> Roles, IReadOnlyList<string> Permissions, bool IsSuperAdmin, DateTimeOffset? CreatedDate = null) : HalResource, IQueryableFields
{
    public static string[] SearchFields => [nameof(FirstName), nameof(LastName), nameof(Email)];
}
