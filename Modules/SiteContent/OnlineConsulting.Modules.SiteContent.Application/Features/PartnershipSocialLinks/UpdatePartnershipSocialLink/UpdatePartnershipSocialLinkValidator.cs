using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.PartnershipSocialLinks.UpdatePartnershipSocialLink;

public class UpdatePartnershipSocialLinkValidator : AbstractValidator<UpdatePartnershipSocialLinkCommand>
{
    public UpdatePartnershipSocialLinkValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(5).MaximumLength(100);
        RuleFor(x => x.Url).NotEmpty().Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute)).WithMessage("'{PropertyName}' must be a valid URL.");
        RuleFor(x => x.Icon).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.IconColor).MaximumLength(7);
    }
}
