using FluentValidation;
using OnlineConsulting.SharedKernel.Payments;

namespace OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.CreateMembershipPlan;

public class CreateMembershipPlanValidator : AbstractValidator<CreateMembershipPlanCommand>
{
    public CreateMembershipPlanValidator()
    {
        _ = RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        _ = RuleFor(x => x.BillingCycle).Must(c => c is BillingCycles.Monthly or BillingCycles.Annual);
        _ = RuleFor(x => x.Price).GreaterThan(0);
        _ = RuleFor(x => x.IncludedVisitsPerYear).GreaterThanOrEqualTo(0);
        _ = RuleFor(x => x.DiscountPercent).InclusiveBetween(0, 100);
        _ = RuleFor(x => x.CreditAmount).GreaterThanOrEqualTo(0);
        _ = RuleFor(x => x.Benefits).MaximumLength(2000);
    }
}
