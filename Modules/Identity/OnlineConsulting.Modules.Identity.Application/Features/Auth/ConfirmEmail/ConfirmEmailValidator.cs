using FluentValidation;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.ConfirmEmail;

public class ConfirmEmailValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailValidator()
    {
        _ = RuleFor(x => x.UserId).NotEmpty();
        _ = RuleFor(x => x.Token).NotEmpty();
    }
}
