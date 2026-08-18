using FluentValidation;
using OnlineConsulting.SharedKernel.Payments;

namespace OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.CreateMembershipPlan;

public class CreateMembershipPlanValidator : AbstractValidator<CreateMembershipPlanCommand>
{
    public CreateMembershipPlanValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.BillingCycle).Must(c => c is BillingCycles.Monthly or BillingCycles.Annual);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.IncludedVisitsPerYear).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DiscountPercent).InclusiveBetween(0, 100);
        RuleFor(x => x.CreditAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Benefits).MaximumLength(2000);
    }
}
