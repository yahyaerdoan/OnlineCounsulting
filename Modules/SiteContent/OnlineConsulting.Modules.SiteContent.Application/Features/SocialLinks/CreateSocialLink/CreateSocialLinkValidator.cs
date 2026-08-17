using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.SocialLinks.CreateSocialLink;

public class CreateSocialLinkValidator : AbstractValidator<CreateSocialLinkCommand>
{
    public CreateSocialLinkValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Url).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Icon).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.IconColor).MaximumLength(7);
    }
}
