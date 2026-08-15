using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.SiteContent.Domain;

public class GalleryCategory : TenantEntity<Guid>
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
