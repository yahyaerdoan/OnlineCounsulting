using FluentValidation;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.FindMyBusiness;

public class FindMyBusinessValidator : AbstractValidator<FindMyBusinessCommand>
{
    public FindMyBusinessValidator()
    {
        _ = RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
