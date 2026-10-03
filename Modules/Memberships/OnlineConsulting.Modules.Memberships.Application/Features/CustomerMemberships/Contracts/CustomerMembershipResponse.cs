using Hateoas;
using OnlineConsulting.Modules.Memberships.Domain;
using OnlineConsulting.SharedKernel.Identity;

namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Contracts;

/// <summary>A class with required init properties instead of a positional record, since records can't inherit LinkedResponse.</summary>
public class CustomerMembershipResponse : LinkedResponse
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }
    public required Guid MembershipPlanId { get; init; }
    public required string Status { get; init; }
    public required DateTimeOffset StartDate { get; init; }
    public DateTimeOffset? RenewalDate { get; init; }
    public required bool CancelAtPeriodEnd { get; init; }
    public DateTimeOffset? TrialEndDate { get; init; }
    public Guid? PromoCodeId { get; init; }
    public string? MemberName { get; init; }
    public string? MemberEmail { get; init; }
    public string? PlanName { get; init; }
    public decimal? PlanPrice { get; init; }
    public string? PlanBillingCycle { get; init; }
    public bool? PlanIsActive { get; init; }

    public static CustomerMembershipResponse FromDomain(CustomerMembership membership) => FromDomain(membership, null, null);

    /// <summary>With the member's contact and the plan filled in, for admin lists and the rejoin card.</summary>
    public static CustomerMembershipResponse FromDomain(CustomerMembership membership, UserContact? member, MembershipPlan? plan) => new()
    {
        MemberName = member?.FullName,
        MemberEmail = member?.Email,
        PlanName = plan?.Name,
        PlanPrice = plan?.Price,
        PlanBillingCycle = plan?.BillingCycle,
        PlanIsActive = plan?.IsActive,
        Id = membership.Id,
        UserId = membership.UserId,
        MembershipPlanId = membership.MembershipPlanId,
        Status = membership.Status,
        StartDate = membership.StartDate,
        RenewalDate = membership.RenewalDate,
        CancelAtPeriodEnd = membership.CancelAtPeriodEnd,
        TrialEndDate = membership.TrialEndDate,
        PromoCodeId = membership.PromoCodeId,
    };
}
