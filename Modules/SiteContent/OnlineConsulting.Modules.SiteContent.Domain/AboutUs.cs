using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.SiteContent.Domain;

public class AboutUs : SequentialGuidTenantEntity
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public string? CoverImage { get; set; }
    public string? VideoUrl { get; set; }
    public int DisplayOrder { get; set; }

    /// <summary>Free-form JSON for template-specific extras, so a new field doesn't need its own migration.</summary>
    public string? Metadata { get; set; }
}
