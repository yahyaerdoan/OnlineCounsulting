using Hateoas;
using OnlineConsulting.Modules.Memberships.Domain;

namespace OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.Contracts;

/// <summary>A class with required init properties instead of a positional record, since records can't inherit LinkedResponse.</summary>
public class MembershipPlanResponse : LinkedResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string BillingCycle { get; init; }
    public required decimal Price { get; init; }
    public required int IncludedVisitsPerYear { get; init; }
    public required decimal DiscountPercent { get; init; }
    public required decimal CreditAmount { get; init; }
    public string? Benefits { get; init; }

    public static MembershipPlanResponse FromDomain(MembershipPlan plan) => new()
    {
        Id = plan.Id,
        Name = plan.Name,
        BillingCycle = plan.BillingCycle,
        Price = plan.Price,
        IncludedVisitsPerYear = plan.IncludedVisitsPerYear,
        DiscountPercent = plan.DiscountPercent,
        CreditAmount = plan.CreditAmount,
        Benefits = plan.Benefits,
    };
}
