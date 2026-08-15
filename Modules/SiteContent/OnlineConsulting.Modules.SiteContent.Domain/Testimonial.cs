using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.SiteContent.Domain;

public class Testimonial : TenantEntity<Guid>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string ImageUrl { get; set; }
    public int DisplayOrder { get; set; }

    /// <summary>Free-form JSON for template-specific extras - keeps this entity from needing a new migration every time a different UI template wants a different field.</summary>
    public string? Metadata { get; set; }
}
