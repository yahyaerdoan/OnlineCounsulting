using FluentValidation;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.CreateTenantAdmin;

/// <summary>Deliberately no email-uniqueness Must() check, unlike RegisterValidator - a duplicate-email check here would be the exact same check-then-act race this command exists to close. UserManager.CreateAsync's own atomic unique index is the real guard; see CreateTenantAdminCommand's doc comment.</summary>
public class CreateTenantAdminValidator : AbstractValidator<CreateTenantAdminCommand>
{
    public CreateTenantAdminValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).+$")
            .WithMessage("Password must contain an uppercase letter, a lowercase letter, a digit and a symbol.");
    }
}
