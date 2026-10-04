using FluentValidation;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Signup.RetryTenantSubscriptionActivation;

public class RetryTenantSubscriptionActivationValidator : AbstractValidator<RetryTenantSubscriptionActivationCommand>
{
    public RetryTenantSubscriptionActivationValidator()
    {
        _ = RuleFor(x => x.TenantId).NotEmpty();
        _ = RuleFor(x => x.PaymentMethodId).NotEmpty();
    }
}
