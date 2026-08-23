using FluentValidation;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.Login;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        _ = RuleFor(x => x.UserNameOrEmail).NotEmpty().WithMessage("Username or email is required.");
        _ = RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
    }
}
