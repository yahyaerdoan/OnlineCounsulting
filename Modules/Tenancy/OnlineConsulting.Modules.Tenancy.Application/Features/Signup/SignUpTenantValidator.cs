using FluentValidation;
using OnlineConsulting.Modules.Tenancy.Application.Features.Signup.Constants;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Signup;

public class SignUpTenantValidator : AbstractValidator<SignUpTenantCommand>
{
    public SignUpTenantValidator()
    {
        RuleFor(x => x.CompanyName).NotEmpty();
        RuleFor(x => x.AdminFirstName).NotEmpty();
        RuleFor(x => x.AdminLastName).NotEmpty();

        RuleFor(x => x.AdminEmail)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.AdminPassword)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).+$")
            .WithMessage("Password must contain an uppercase letter, a lowercase letter, a digit and a symbol.");

        RuleFor(x => x.ModuleKeys)
            .NotEmpty()
            .WithMessage(SignupMessages.NoModulesSelected);

        RuleFor(x => x.PaymentMethodId).NotEmpty();
    }
}
