using FluentValidation;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.Abstractions;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth;

public abstract class TenantAdminFieldsValidator<T> : AbstractValidator<T> where T : ITenantAdminFields
{
    protected TenantAdminFieldsValidator()
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
