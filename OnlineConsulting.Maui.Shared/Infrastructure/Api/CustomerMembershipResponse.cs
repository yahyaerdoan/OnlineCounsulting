
using OnlineConsulting.Maui.Shared.Infrastructure.Hateoas;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/v1/memberships/query's response shape; the Member*/Plan* fields are filled by the admin list and the rejoin lookup.</summary>
public record CustomerMembershipResponse(Guid Id, Guid UserId, Guid MembershipPlanId, string Status, DateTimeOffset StartDate, DateTimeOffset? RenewalDate, bool CancelAtPeriodEnd,
    DateTimeOffset? TrialEndDate = null, Guid? PromoCodeId = null, string? MemberName = null, string? MemberEmail = null, string? PlanName = null,
    decimal? PlanPrice = null, string? PlanBillingCycle = null, bool? PlanIsActive = null) : HalResource, IQueryableFields
{
    public static string[] SearchFields => [];

    public bool IsEnding => CancelAtPeriodEnd && Status != "Cancelled";
}
