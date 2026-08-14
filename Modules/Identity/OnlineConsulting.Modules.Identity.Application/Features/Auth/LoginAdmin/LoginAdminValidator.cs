using FluentValidation;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.LoginAdmin;

public class LoginAdminValidator : AbstractValidator<LoginAdminCommand>
{
    public LoginAdminValidator()
    {
        RuleFor(x => x.UserNameOrEmail).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}
