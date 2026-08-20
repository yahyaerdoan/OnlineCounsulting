using FluentValidation;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.Subscribe;

public class SubscribeNewsletterValidator : AbstractValidator<SubscribeNewsletterCommand>
{
    public SubscribeNewsletterValidator()
    {
        _ = RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(320);
    }
}
