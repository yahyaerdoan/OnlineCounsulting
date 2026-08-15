using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.SiteContent.Domain;

/// <summary>A step in the "how you get our service" homepage section. Renamed from the legacy HowIGetService for clarity, same rationale as WhatWeProvide -> FeatureHighlight.</summary>
public class ServiceProcessStep : TenantEntity<Guid>
{
    public required string Title { get; set; }
    public required string Description { get; set; }

    /// <summary>MudBlazor icon value - replaces the legacy ImgIcon FK/upload (Category.Icon precedent).</summary>
    public required string Icon { get; set; }
    public string? IconColor { get; set; }

    public int DisplayOrder { get; set; }

    /// <summary>Free-form JSON for template-specific extras (SiteContent convention).</summary>
    public string? Metadata { get; set; }
}
