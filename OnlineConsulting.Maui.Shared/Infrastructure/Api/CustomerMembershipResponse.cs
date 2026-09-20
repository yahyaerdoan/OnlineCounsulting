namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/memberships/query's response shape.</summary>
public record CustomerMembershipResponse(Guid Id, Guid UserId, Guid MembershipPlanId, string Status, DateTimeOffset StartDate, DateTimeOffset? RenewalDate, bool CancelAtPeriodEnd, DateTimeOffset? TrialEndDate = null, Guid? PromoCodeId = null) : IQueryableFields
{
    public static string[] SearchFields => [nameof(Status)];
}
