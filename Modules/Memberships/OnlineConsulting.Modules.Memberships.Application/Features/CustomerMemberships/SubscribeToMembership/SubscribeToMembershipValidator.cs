using FluentValidation;

namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.SubscribeToMembership;

public class SubscribeToMembershipValidator : AbstractValidator<SubscribeToMembershipCommand>
{
    public SubscribeToMembershipValidator()
    {
        _ = RuleFor(x => x.MembershipPlanId).NotEmpty();
        _ = RuleFor(x => x.PaymentMethodId).NotEmpty();
        _ = RuleFor(x => x.CreditToApplyAmount).GreaterThan(0).When(x => x.CreditToApplyAmount is not null);
    }
}
