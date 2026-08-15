using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.SiteContent.Domain;

/// <summary>Renamed from the legacy WhatWeProvide - "feature highlight" is the standard term for this marketing-site content block (image + title + description callout).</summary>
public class FeatureHighlight : TenantEntity<Guid>
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string ImageUrl { get; set; }
    public int DisplayOrder { get; set; }

    /// <summary>Free-form JSON for template-specific extras - keeps this entity from needing a new migration every time a different UI template wants a different field.</summary>
    public string? Metadata { get; set; }
}
