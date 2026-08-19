using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.SiteContent.Domain.Service;

/// <summary>A card in the "what we provide" homepage section. Renamed from the legacy ProvidedItem for clarity, same rationale as WhatWeProvide -> FeatureHighlight. Independent of ServiceProcessStep - same shape, different content type, no relationship between them.</summary>
public class ServiceOffering : TenantEntity<Guid>
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
