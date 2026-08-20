using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.SocialLinks.UpdateSocialLink;

public class UpdateSocialLinkValidator : AbstractValidator<UpdateSocialLinkCommand>
{
    public UpdateSocialLinkValidator()
    {
        _ = RuleFor(x => x.Id).NotEmpty();
        _ = RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        _ = RuleFor(x => x.Url).NotEmpty().MaximumLength(500);
        _ = RuleFor(x => x.Icon).NotEmpty().MaximumLength(2000);
        _ = RuleFor(x => x.IconColor).MaximumLength(7);
    }
}
