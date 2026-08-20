using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.PartnershipSocialLinks.CreatePartnershipSocialLink;

public class CreatePartnershipSocialLinkValidator : AbstractValidator<CreatePartnershipSocialLinkCommand>
{
    public CreatePartnershipSocialLinkValidator()
    {
        _ = RuleFor(x => x.PartnershipId).NotEmpty();
        _ = RuleFor(x => x.Name).NotEmpty().MinimumLength(5).MaximumLength(100);
        _ = RuleFor(x => x.Url).NotEmpty().Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute)).WithMessage("'{PropertyName}' must be a valid URL.");
        _ = RuleFor(x => x.Icon).NotEmpty().MaximumLength(2000);
        _ = RuleFor(x => x.IconColor).MaximumLength(7);
    }
}
