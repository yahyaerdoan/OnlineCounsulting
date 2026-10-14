using FluentValidation;
using OnlineConsulting.Modules.SiteContent.Application.Common;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.LandingPages.SaveLandingPage;

public class SaveLandingPageValidator : AbstractValidator<SaveLandingPageCommand>
{
    public SaveLandingPageValidator()
    {
        _ = RuleFor(x => x.HeroEyebrow).NotEmpty().MaximumLength(LandingLimits.Eyebrow);
        _ = RuleFor(x => x.HeroTitle).NotEmpty().MaximumLength(LandingLimits.Title);
        _ = RuleFor(x => x.HeroLead).NotEmpty().MaximumLength(LandingLimits.Lead);
        _ = RuleFor(x => x.HeroImageUrl).MaximumLength(LandingLimits.ImageUrl);
        _ = RuleFor(x => x.FeaturesHeroImageUrl).MaximumLength(LandingLimits.ImageUrl);
        _ = RuleFor(x => x.IndustriesHeroImageUrl).MaximumLength(LandingLimits.ImageUrl);
        _ = RuleFor(x => x.AboutHeroImageUrl).MaximumLength(LandingLimits.ImageUrl);
        _ = RuleFor(x => x.ContactHeroImageUrl).MaximumLength(LandingLimits.ImageUrl);
        _ = RuleFor(x => x.PrimaryCtaText).NotEmpty().MaximumLength(LandingLimits.Cta);
        _ = RuleFor(x => x.SecondaryCtaText).NotEmpty().MaximumLength(LandingLimits.Cta);

        _ = RuleFor(x => x.FeaturesEyebrow).NotEmpty().MaximumLength(LandingLimits.Eyebrow);
        _ = RuleFor(x => x.FeaturesTitle).NotEmpty().MaximumLength(LandingLimits.Title);
        _ = RuleFor(x => x.FeaturesLead).MaximumLength(LandingLimits.Lead);

        _ = RuleFor(x => x.StepsEyebrow).NotEmpty().MaximumLength(LandingLimits.Eyebrow);
        _ = RuleFor(x => x.StepsTitle).NotEmpty().MaximumLength(LandingLimits.Title);
        _ = RuleFor(x => x.StepsLead).MaximumLength(LandingLimits.Lead);

        _ = RuleFor(x => x.PricingEyebrow).NotEmpty().MaximumLength(LandingLimits.Eyebrow);
        _ = RuleFor(x => x.PricingTitle).NotEmpty().MaximumLength(LandingLimits.Title);
        _ = RuleFor(x => x.PricingLead).MaximumLength(LandingLimits.Lead);

        _ = RuleFor(x => x.FaqEyebrow).NotEmpty().MaximumLength(LandingLimits.Eyebrow);
        _ = RuleFor(x => x.FaqTitle).NotEmpty().MaximumLength(LandingLimits.Title);

        _ = RuleFor(x => x.ClosingTitle).NotEmpty().MaximumLength(LandingLimits.Title);
        _ = RuleFor(x => x.ClosingLead).MaximumLength(LandingLimits.Lead);

        _ = RuleFor(x => x.AboutHeroEyebrow).MaximumLength(LandingLimits.Eyebrow);
        _ = RuleFor(x => x.AboutHeroTitle).MaximumLength(LandingLimits.Title);
        _ = RuleFor(x => x.AboutHeroLead).MaximumLength(LandingLimits.Lead);

        _ = RuleFor(x => x.IndustriesEyebrow).MaximumLength(LandingLimits.Eyebrow);
        _ = RuleFor(x => x.IndustriesTitle).MaximumLength(LandingLimits.Title);
        _ = RuleFor(x => x.IndustriesLead).MaximumLength(LandingLimits.Lead);

        _ = RuleFor(x => x.ContactHeroEyebrow).MaximumLength(LandingLimits.Eyebrow);
        _ = RuleFor(x => x.ContactHeroTitle).MaximumLength(LandingLimits.Title);
        _ = RuleFor(x => x.ContactHeroLead).MaximumLength(LandingLimits.Lead);
    }
}
