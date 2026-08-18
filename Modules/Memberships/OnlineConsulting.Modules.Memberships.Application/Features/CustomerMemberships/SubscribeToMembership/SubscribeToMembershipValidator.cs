using FluentValidation;

namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.SubscribeToMembership;

public class SubscribeToMembershipValidator : AbstractValidator<SubscribeToMembershipCommand>
{
    public SubscribeToMembershipValidator()
    {
        RuleFor(x => x.MembershipPlanId).NotEmpty();
        RuleFor(x => x.PaymentMethodId).NotEmpty();
        RuleFor(x => x.CreditToApplyAmount).GreaterThan(0).When(x => x.CreditToApplyAmount is not null);
    }
}
