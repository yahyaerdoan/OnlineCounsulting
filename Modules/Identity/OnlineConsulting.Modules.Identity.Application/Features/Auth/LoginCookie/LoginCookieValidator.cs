using FluentValidation;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.LoginCookie;

public class LoginCookieValidator : AbstractValidator<LoginCookieCommand>
{
    public LoginCookieValidator()
    {
        RuleFor(x => x.UserNameOrEmail).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}
