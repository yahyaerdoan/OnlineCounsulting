namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/memberships/query's response shape.</summary>
public record CustomerMembershipResponse(Guid Id, Guid UserId, Guid MembershipPlanId, string Status, DateTimeOffset StartDate, DateTimeOffset? RenewalDate) : IQueryableFields
{
    public static string[] SearchFields => [nameof(Status)];
}
