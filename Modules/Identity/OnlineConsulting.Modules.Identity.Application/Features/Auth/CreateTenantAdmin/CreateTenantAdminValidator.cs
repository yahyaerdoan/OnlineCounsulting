using FluentValidation;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.CreateTenantAdmin;

/// <summary>No email-uniqueness check here (unlike RegisterValidator) - UserManager.CreateAsync's atomic unique index is the real guard, avoiding a check-then-act race.</summary>
public class CreateTenantAdminValidator : AbstractValidator<CreateTenantAdminCommand>
{
    public CreateTenantAdminValidator()
    {
        _ = RuleFor(x => x.FirstName).NotEmpty();
        _ = RuleFor(x => x.LastName).NotEmpty();

        _ = RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        _ = RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).+$")
            .WithMessage("Password must contain an uppercase letter, a lowercase letter, a digit and a symbol.");
    }
}
