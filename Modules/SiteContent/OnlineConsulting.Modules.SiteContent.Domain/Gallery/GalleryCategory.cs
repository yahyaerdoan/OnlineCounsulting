using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.SiteContent.Domain.Gallery;

public class GalleryCategory : SequentialGuidTenantEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
