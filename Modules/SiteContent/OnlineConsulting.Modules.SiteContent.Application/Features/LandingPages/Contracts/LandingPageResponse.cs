using OnlineConsulting.Modules.SiteContent.Domain.Landing;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.LandingPages.Contracts;

public record LandingPageResponse(
    string HeroEyebrow, string HeroTitle, string HeroLead, string PrimaryCtaText, string SecondaryCtaText,
    string FeaturesEyebrow, string FeaturesTitle, string? FeaturesLead,
    string StepsEyebrow, string StepsTitle, string? StepsLead,
    string PricingEyebrow, string PricingTitle, string? PricingLead,
    string FaqEyebrow, string FaqTitle,
    string ClosingTitle, string? ClosingLead,
    string? AboutHeroEyebrow = null, string? AboutHeroTitle = null, string? AboutHeroLead = null,
    string? IndustriesEyebrow = null, string? IndustriesTitle = null, string? IndustriesLead = null,
    string? ContactHeroEyebrow = null, string? ContactHeroTitle = null, string? ContactHeroLead = null,
    string? HeroImageUrl = null, string? FeaturesHeroImageUrl = null, string? IndustriesHeroImageUrl = null,
    string? AboutHeroImageUrl = null, string? ContactHeroImageUrl = null)
{
    public static LandingPageResponse FromDomain(LandingPage entity) => new(
        entity.HeroEyebrow, entity.HeroTitle, entity.HeroLead, entity.PrimaryCtaText, entity.SecondaryCtaText,
        entity.FeaturesEyebrow, entity.FeaturesTitle, entity.FeaturesLead,
        entity.StepsEyebrow, entity.StepsTitle, entity.StepsLead,
        entity.PricingEyebrow, entity.PricingTitle, entity.PricingLead,
        entity.FaqEyebrow, entity.FaqTitle,
        entity.ClosingTitle, entity.ClosingLead,
        entity.AboutHeroEyebrow, entity.AboutHeroTitle, entity.AboutHeroLead,
        entity.IndustriesEyebrow, entity.IndustriesTitle, entity.IndustriesLead,
        entity.ContactHeroEyebrow, entity.ContactHeroTitle, entity.ContactHeroLead,
        entity.HeroImageUrl, entity.FeaturesHeroImageUrl, entity.IndustriesHeroImageUrl,
        entity.AboutHeroImageUrl, entity.ContactHeroImageUrl);
}
