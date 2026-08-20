using FluentValidation;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Domain;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.Register;

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator(UserManager<User> userManager)
    {
        _ = RuleFor(x => x.FirstName).NotEmpty();
        _ = RuleFor(x => x.LastName).NotEmpty();

        _ = RuleFor(x => x.UserName)
            .NotEmpty()
            // Sync, not async: FluentValidation's implicit ASP.NET pipeline only supports sync rules.
            .Must(userName => !userManager.Users.Any(u => u.NormalizedUserName == userName.ToUpperInvariant()))
            .WithMessage("This username is already taken.");

        _ = RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .Must(email => !userManager.Users.Any(u => u.NormalizedEmail == email.ToUpperInvariant()))
            .WithMessage("This email is already registered.");

        _ = RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).+$")
            .WithMessage("Password must contain an uppercase letter, a lowercase letter, a digit and a symbol.");
    }
}
