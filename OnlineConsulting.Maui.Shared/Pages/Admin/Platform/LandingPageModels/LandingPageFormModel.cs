namespace OnlineConsulting.Maui.Shared.Pages.Admin.Platform.LandingPageModels;

/// <summary>Bound by LandingPageFormPage; one row, created on first save.</summary>
public class LandingPageFormModel
{
    public string HeroEyebrow { get; set; } = string.Empty;
    public string HeroTitle { get; set; } = string.Empty;
    public string HeroLead { get; set; } = string.Empty;
    public string PrimaryCtaText { get; set; } = string.Empty;
    public string SecondaryCtaText { get; set; } = string.Empty;
    public string? HeroImageUrl { get; set; }
    public string? FeaturesHeroImageUrl { get; set; }
    public string? IndustriesHeroImageUrl { get; set; }
    public string? AboutHeroImageUrl { get; set; }
    public string? ContactHeroImageUrl { get; set; }

    public string FeaturesEyebrow { get; set; } = string.Empty;
    public string FeaturesTitle { get; set; } = string.Empty;
    public string? FeaturesLead { get; set; }

    public string StepsEyebrow { get; set; } = string.Empty;
    public string StepsTitle { get; set; } = string.Empty;
    public string? StepsLead { get; set; }

    public string PricingEyebrow { get; set; } = string.Empty;
    public string PricingTitle { get; set; } = string.Empty;
    public string? PricingLead { get; set; }

    public string FaqEyebrow { get; set; } = string.Empty;
    public string FaqTitle { get; set; } = string.Empty;

    public string ClosingTitle { get; set; } = string.Empty;
    public string? ClosingLead { get; set; }

    public string AboutHeroEyebrow { get; set; } = string.Empty;
    public string AboutHeroTitle { get; set; } = string.Empty;
    public string? AboutHeroLead { get; set; }

    public string IndustriesEyebrow { get; set; } = string.Empty;
    public string IndustriesTitle { get; set; } = string.Empty;
    public string? IndustriesLead { get; set; }

    public string ContactHeroEyebrow { get; set; } = string.Empty;
    public string ContactHeroTitle { get; set; } = string.Empty;
    public string? ContactHeroLead { get; set; }

    public void Load(Infrastructure.Api.LandingPageResponse page)
    {
        HeroEyebrow = page.HeroEyebrow;
        HeroTitle = page.HeroTitle;
        HeroLead = page.HeroLead;
        PrimaryCtaText = page.PrimaryCtaText;
        SecondaryCtaText = page.SecondaryCtaText;
        HeroImageUrl = page.HeroImageUrl;
        FeaturesHeroImageUrl = page.FeaturesHeroImageUrl;
        IndustriesHeroImageUrl = page.IndustriesHeroImageUrl;
        AboutHeroImageUrl = page.AboutHeroImageUrl;
        ContactHeroImageUrl = page.ContactHeroImageUrl;
        FeaturesEyebrow = page.FeaturesEyebrow;
        FeaturesTitle = page.FeaturesTitle;
        FeaturesLead = page.FeaturesLead;
        StepsEyebrow = page.StepsEyebrow;
        StepsTitle = page.StepsTitle;
        StepsLead = page.StepsLead;
        PricingEyebrow = page.PricingEyebrow;
        PricingTitle = page.PricingTitle;
        PricingLead = page.PricingLead;
        FaqEyebrow = page.FaqEyebrow;
        FaqTitle = page.FaqTitle;
        ClosingTitle = page.ClosingTitle;
        ClosingLead = page.ClosingLead;
        AboutHeroEyebrow = page.AboutHeroEyebrow ?? string.Empty;
        AboutHeroTitle = page.AboutHeroTitle ?? string.Empty;
        AboutHeroLead = page.AboutHeroLead;
        IndustriesEyebrow = page.IndustriesEyebrow ?? string.Empty;
        IndustriesTitle = page.IndustriesTitle ?? string.Empty;
        IndustriesLead = page.IndustriesLead;
        ContactHeroEyebrow = page.ContactHeroEyebrow ?? string.Empty;
        ContactHeroTitle = page.ContactHeroTitle ?? string.Empty;
        ContactHeroLead = page.ContactHeroLead;
    }
}
