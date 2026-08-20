using FluentValidation;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Signup;

public class ActivateTenantSubscriptionValidator : AbstractValidator<ActivateTenantSubscriptionCommand>
{
    public ActivateTenantSubscriptionValidator()
    {
        _ = RuleFor(x => x.TenantId).NotEmpty();
        _ = RuleFor(x => x.PaymentMethodId).NotEmpty();
    }
}
