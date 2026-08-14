using FluentValidation;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.Login;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.UserNameOrEmail).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}
