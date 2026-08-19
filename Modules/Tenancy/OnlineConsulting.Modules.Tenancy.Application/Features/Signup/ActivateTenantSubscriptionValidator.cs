using FluentValidation;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Signup;

public class ActivateTenantSubscriptionValidator : AbstractValidator<ActivateTenantSubscriptionCommand>
{
    public ActivateTenantSubscriptionValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.PaymentMethodId).NotEmpty();
    }
}
