using Hateoas;
using OnlineConsulting.Modules.Memberships.Domain;

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

    public static CustomerMembershipResponse FromDomain(CustomerMembership membership) => new()
    {
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
