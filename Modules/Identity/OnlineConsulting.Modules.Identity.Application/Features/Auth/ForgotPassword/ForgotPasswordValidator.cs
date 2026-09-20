using FluentValidation;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.ForgotPassword;

public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordValidator()
    {
        _ = RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
