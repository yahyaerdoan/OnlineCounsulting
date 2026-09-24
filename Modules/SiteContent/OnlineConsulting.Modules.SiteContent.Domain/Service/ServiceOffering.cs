using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.SiteContent.Domain.Service;

/// <summary>A card in the "what we provide" homepage section; renamed from the legacy ProvidedItem, unrelated to ServiceProcessStep despite the similar shape.</summary>
public class ServiceOffering : SequentialGuidTenantEntity
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
