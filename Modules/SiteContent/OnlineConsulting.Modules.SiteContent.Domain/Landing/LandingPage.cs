using Core.PersistenceLayer.MultiTenancy;

namespace OnlineConsulting.Modules.SiteContent.Domain.Landing;

/// <summary>Platform landing copy, one row per site; only the platform's own site renders it.</summary>
public class LandingPage : SequentialGuidTenantEntity
{
    public required string HeroEyebrow { get; set; }
    public required string HeroTitle { get; set; }
    public required string HeroLead { get; set; }
    public required string PrimaryCtaText { get; set; }
    public required string SecondaryCtaText { get; set; }

    public required string FeaturesEyebrow { get; set; }
    public required string FeaturesTitle { get; set; }
    public string? FeaturesLead { get; set; }

    public required string StepsEyebrow { get; set; }
    public required string StepsTitle { get; set; }
    public string? StepsLead { get; set; }

    public required string PricingEyebrow { get; set; }
    public required string PricingTitle { get; set; }
    public string? PricingLead { get; set; }

    public required string FaqEyebrow { get; set; }
    public required string FaqTitle { get; set; }

    public required string ClosingTitle { get; set; }
    public string? ClosingLead { get; set; }

    /// <summary>About page hero; the plain page banner shows while the title is empty.</summary>
    public string? AboutHeroEyebrow { get; set; }
    public string? AboutHeroTitle { get; set; }
    public string? AboutHeroLead { get; set; }

    /// <summary>Industries grid (from categories); hidden while the title is empty.</summary>
    public string? IndustriesEyebrow { get; set; }
    public string? IndustriesTitle { get; set; }
    public string? IndustriesLead { get; set; }

    /// <summary>Contact page hero; the plain page banner shows while the title is empty.</summary>
    public string? ContactHeroEyebrow { get; set; }
    public string? ContactHeroTitle { get; set; }
    public string? ContactHeroLead { get; set; }

    /// <summary>Landing hero picture beside the copy; none keeps the centered hero.</summary>
    public string? HeroImageUrl { get; set; }

    /// <summary>Hero pictures of the Features, Industries, About and Contact pages.</summary>
    public string? FeaturesHeroImageUrl { get; set; }
    public string? IndustriesHeroImageUrl { get; set; }
    public string? AboutHeroImageUrl { get; set; }
    public string? ContactHeroImageUrl { get; set; }
}
