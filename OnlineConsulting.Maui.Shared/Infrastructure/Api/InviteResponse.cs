using OnlineConsulting.Maui.Shared.Infrastructure.Hateoas;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/v1/invites/query's response shape.</summary>
public record InviteResponse(Guid Id, string Email, string RoleName, string Status, DateTime ExpiresAt, DateTimeOffset CreatedDate)
    : HalResource, IQueryableFields
{
    public static string[] SearchFields => [nameof(Email)];
}
