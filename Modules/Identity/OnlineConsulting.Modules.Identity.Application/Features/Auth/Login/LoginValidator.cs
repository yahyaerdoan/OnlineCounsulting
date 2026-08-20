using FluentValidation;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.Login;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        _ = RuleFor(x => x.UserNameOrEmail).NotEmpty();
        _ = RuleFor(x => x.Password).NotEmpty();
    }
}
