namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors GET /api/v1/site-content/landing-page.</summary>
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
    string? AboutHeroImageUrl = null, string? ContactHeroImageUrl = null);
