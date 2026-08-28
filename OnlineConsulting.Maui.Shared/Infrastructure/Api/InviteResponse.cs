namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/invites/query's response shape.</summary>
public record InviteResponse(Guid Id, string Email, string RoleName, string Status, DateTime ExpiresAt, DateTimeOffset CreatedDate)
    : IQueryableFields
{
    public static string[] SearchFields => [nameof(Email)];
}
