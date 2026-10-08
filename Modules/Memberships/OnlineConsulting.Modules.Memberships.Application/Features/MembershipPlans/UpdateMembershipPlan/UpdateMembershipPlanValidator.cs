using FluentValidation;

namespace OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.UpdateMembershipPlan;

public class UpdateMembershipPlanValidator : AbstractValidator<UpdateMembershipPlanCommand>
{
    public UpdateMembershipPlanValidator()
    {
        _ = RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        _ = RuleFor(x => x.IncludedVisitsPerYear).GreaterThanOrEqualTo(0);
        _ = RuleFor(x => x.DiscountPercent).InclusiveBetween(0, 100);
        _ = RuleFor(x => x.CreditAmount).GreaterThanOrEqualTo(0);
        _ = RuleFor(x => x.Benefits).MaximumLength(2000);
    }
}
