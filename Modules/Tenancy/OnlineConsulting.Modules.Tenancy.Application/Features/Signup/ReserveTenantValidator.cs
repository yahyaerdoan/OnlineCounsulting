using FluentValidation;
using OnlineConsulting.Modules.Tenancy.Application.Features.Signup.Constants;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Signup;

public class ReserveTenantValidator : AbstractValidator<ReserveTenantCommand>
{
    public ReserveTenantValidator()
    {
        RuleFor(x => x.CompanyName).NotEmpty();

        RuleFor(x => x.AdminEmail)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.ModuleKeys)
            .NotEmpty()
            .WithMessage(SignupMessages.NoModulesSelected);
    }
}
