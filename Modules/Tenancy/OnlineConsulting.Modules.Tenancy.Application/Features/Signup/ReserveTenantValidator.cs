using FluentValidation;
using OnlineConsulting.Modules.Tenancy.Application.Features.Signup.Constants;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Signup;

public class ReserveTenantValidator : AbstractValidator<ReserveTenantCommand>
{
    public ReserveTenantValidator()
    {
        _ = RuleFor(x => x.CompanyName).NotEmpty();

        _ = RuleFor(x => x.AdminEmail)
            .NotEmpty()
            .EmailAddress();

        _ = RuleFor(x => x.ModuleKeys)
            .NotEmpty()
            .WithMessage(SignupMessages.NoModulesSelected);
    }
}
